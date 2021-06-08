using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.IService;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Domain.Teams;
using Domain.Tournaments;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly CodingChainContext _context;
        private readonly ITimeService _timeService;

        public TeamRepository(CodingChainContext context, ITimeService timeService)
        {
            _context = context;
            _timeService = timeService;
        }

        public async Task<TeamId> SetAsync(TeamAggregate aggregate)
        {
            var team = await ToModel(aggregate);
            _context.Teams.Upsert(team);
            await _context.SaveChangesAsync();
            return new TeamId(team.Id);
        }

        public async Task<TeamAggregate?> FindByIdAsync(TeamId id)
        {
            var team = await GetIncludeQuery(_context.Teams)
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id.Value);
            return team is not null ? await ToEntity(team) : null;
        }

        public async Task RemoveAsync(TeamId id)
        {
            var team = await _context.Teams
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id.Value);
            if (team is not null)
                team.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public Task<TeamId> NextIdAsync()
        {
            return new TeamId(Guid.NewGuid()).ToTask();
        }

        private async Task<Team> ToModel(TeamAggregate aggregate)
        {
            var team = await GetTeam(aggregate);
            var leavingMembers = GetLeavingMembers(aggregate, team);
            var currentMembers = GetCurrentMembers(team);
            var newMembers = GetNewMembers(aggregate, currentMembers);
            var leavingParticipations = GetLeavingParticipations(aggregate, team);
            UpdateMembers(aggregate, currentMembers);
            newMembers.ForEach(uT => team.UserTeams.Add(uT));
            leavingMembers.ForEach(uT => uT.LeaveDate = _timeService.Now());
            leavingParticipations.ForEach(p => p.Deactivated = true);

            team.Name = aggregate.Name;
            return team;
        }

        private List<Participation> GetLeavingParticipations(TeamAggregate aggregate, Team team)
        {
            return team.ActiveParticipations
                .Where(p => aggregate.TournamentIds.All(t => t.Value != p.Tournament.Id))
                .ToList();
        }

        private static void UpdateMembers(TeamAggregate aggregate, List<UserTeam> currentMembers)
        {
            currentMembers.ForEach(uT =>
            {
                var member = aggregate.Members.FirstOrDefault(m => m.Id.Value == uT.UserId);
                uT.IsAdmin = member?.IsAdmin ?? uT.IsAdmin;
            });
        }

        private async Task<Team> GetTeam(TeamAggregate aggregate)
        {
            return await GetIncludeQuery(_context.Teams)
                       .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == aggregate.Id.Value) ??
                   new Team {Id = aggregate.Id.Value};
        }

        private static List<UserTeam> GetLeavingMembers(TeamAggregate aggregate, Team team)
        {
            return team.UserTeams
                .Where(uT => uT.LeaveDate == null && aggregate.Members.All(m => m.Id.Value != uT.UserId))
                .ToList();
        }

        private static List<UserTeam> GetCurrentMembers(Team team)
        {
            return team.UserTeams
                .Where(uT => uT.LeaveDate == null && !uT.User.IsDeleted)
                .ToList();
        }

        private List<UserTeam> GetNewMembers(TeamAggregate aggregate, List<UserTeam> currentMembers)
        {
            var newMembers = aggregate.Members
                .Where(m => currentMembers.All(uT => uT.UserId != m.Id.Value))
                .Select(m => new UserTeam {IsAdmin = m.IsAdmin, JoinDate = _timeService.Now(), UserId = m.Id.Value})
                .ToList();
            return newMembers;
        }

        private Task<TeamAggregate> ToEntity(Team team)
        {
            var aggregate = TeamAggregate.Restore(
                new TeamId(team.Id),
                team.Name,
                team.UserTeams
                    .Where(uT => uT.LeaveDate == null && !uT.User.IsDeleted)
                    .Select(uT => new MemberEntity(new UserId(uT.UserId), uT.IsAdmin))
                    .ToList(),
                team.ActiveParticipations
                    .Select(p => new TournamentId(p.Tournament.Id))
                    .Distinct()
                    .ToList()
            );
            return aggregate.ToTask();
        }

        private static IQueryable<Team> GetIncludeQuery(IQueryable<Team> teams)
        {
            return teams
                .Include(t => t.UserTeams)
                .ThenInclude<Team, UserTeam, User>(uT => uT.User)
                .Include(t => t.Participations)
                .ThenInclude<Team, Participation, Tournament>(p => p.Tournament)
                .Include(t => t.Participations)
                .ThenInclude<Team, Participation, Step>(p => p.Step);
        }
    }
}