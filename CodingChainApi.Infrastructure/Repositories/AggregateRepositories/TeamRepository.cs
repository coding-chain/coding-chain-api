using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.IService;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Domain.Teams;
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

        private async Task<Team> ToModel(TeamAggregate aggregate)
        {
            var team = await _context.Teams
                .Include(t => t.UserTeams)
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == aggregate.Id.Value) ?? new Team();
            var leavingMembers = team.UserTeams
                .Where(uT => uT.LeaveDate == null && aggregate.Members.All(m => m.Id.Value != uT.UserId))
                .ToList();
            var currentMembers = team.UserTeams
                .Where(uT => uT.LeaveDate == null && !uT.User.IsDeleted)
                .ToList();
            var newMembers = aggregate.Members
                .Where(m => currentMembers.All(uT => uT.UserId != m.Id.Value))
                .Select(m => new UserTeam {IsAdmin = m.IsAdmin, JoinDate = _timeService.Now(), UserId = m.Id.Value})
                .ToList();
            currentMembers.ForEach(uT =>
            {
                var member = aggregate.Members.FirstOrDefault(m => m.Id.Value == uT.UserId);
                uT.IsAdmin = member?.IsAdmin ?? uT.IsAdmin;
            });
            
            newMembers.ForEach(uT => team.UserTeams.Add(uT));
            leavingMembers.ForEach(uT => uT.LeaveDate = _timeService.Now());

            team.Name = aggregate.Name;
            return team;
        }

        private Task<TeamAggregate> ToEntity(Team team)
        {
            var aggregate = new TeamAggregate(
                new TeamId(team.Id),
                team.Name,
                team.UserTeams
                    .Where(uT => uT.LeaveDate == null && !uT.User.IsDeleted)
                    .Select(uT => new MemberEntity(new UserId(uT.UserId), uT.IsAdmin))
                    .ToList()
            );
            return aggregate.ToTask();
        }

        public async Task<TeamId> SetAsync(TeamAggregate aggregate)
        {
            var team = await ToModel(aggregate);
            _context.Teams.Update(team);
            await _context.SaveChangesAsync();
            return new TeamId(team.Id);
        }

        public async Task<TeamAggregate?> FindByIdAsync(TeamId id)
        {
            var team = await _context.Teams
                .Include(t => t.UserTeams)
                .ThenInclude(uT => uT.User)
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id.Value);
            return team is not null ? await ToEntity(team) : null;
        }

        public async Task RemoveAsync(TeamId id)
        {
            var team = await _context.Teams
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id.Value);
            team.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public Task<TeamId> NextIdAsync()
        {
            return new TeamId(Guid.NewGuid()).ToTask();
        }
    }
}