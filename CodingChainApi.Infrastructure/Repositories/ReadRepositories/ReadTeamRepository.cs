using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Teams;
using Application.Read.Teams.Handlers;
using Application.Read.Tournaments;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadTeamRepository : IReadTeamRepository
    {
        private readonly CodingChainContext _context;

        public ReadTeamRepository(CodingChainContext context)
        {
            _context = context;
        }

        private static TeamNavigation ToTeamNavigation(Team team) => new(
            team.Id,
            team.Name,
            team.ActiveMembersIds,
            team.ActiveParticipationsIds
        );

        private static Expression<Func<Team, bool>> ToPredicate(GetTeamNavigationPaginatedQuery query) =>
            team =>
                !team.IsDeleted
                && (query.MemberIdFilter == null || team.UserTeams.Any(uT =>
                    uT.LeaveDate == null && !uT.User.IsDeleted && uT.User.Id == query.MemberIdFilter))
                && (query.NameFilter == null || team.Name.Contains(query.NameFilter));

        private static IQueryable<Team> GetOrderByQuery(IQueryable<Team> teamQuery,
            GetTeamNavigationPaginatedQuery paginationQuery)
        {
            if (paginationQuery.NameOrder == OrderEnum.Asc)
                return teamQuery.OrderBy(t => t.Name);
            return teamQuery.OrderByDescending(t => t.Name);
        }

        public async Task<IPagedList<TeamNavigation>> GetAllTeamNavigationPaginated(
            GetTeamNavigationPaginatedQuery paginationQuery)
        {
            var query = GetTeamIncludeQueryable()
                .Where(ToPredicate(paginationQuery));
            if (paginationQuery.NameOrder != null)
            {
                query = GetOrderByQuery(query, paginationQuery);
            }

            return await query.Select(t => ToTeamNavigation(t))
                .FromPaginationQueryAsync(paginationQuery);
        }


        public async Task<TeamNavigation?> GetOneTeamNavigationByIdAsync(Guid id)
        {
            var team = await GetTeamIncludeQueryable()
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id);
            if (team is null) return null;
            return ToTeamNavigation(team);
        }

        private IIncludableQueryable<Team, User> GetTeamIncludeQueryable()
        {
            return _context.Teams
                .Include(t => t.UserTeams)
                .Include(t => t.Participations)
                .ThenInclude(p => p.Tournament)
                .Include(t => t.Participations)
                .ThenInclude(p => p.Step)
                .Include(t => t.UserTeams)
                .ThenInclude(uT => uT.User);
        }

        public async Task<MemberNavigation?> GetOneMemberNavigationByIdAsync(Guid teamId, Guid userId)
        {
            var member = await GetUserTeamIncludeQueryable()
                .FirstOrDefaultAsync(uT =>
                    uT.TeamId == teamId && uT.UserId == userId && !uT.Team.IsDeleted && !uT.User.IsDeleted &&
                    uT.LeaveDate == null);
            return member is null ? null : ToMemberNavigation(member);
        }

        public async Task<IList<Guid>> GetTeamMembersIds(Guid teamId)
        {
            return await GetUserTeamIncludeQueryable()
                .Where(uT => !uT.Team.IsDeleted && !uT.User.IsDeleted && uT.LeaveDate == null)
                .Select(uT => uT.Id)
                .ToListAsync();
        }

        public Task<bool> TeamExistsById(Guid id)
        {
            return _context.Teams.AnyAsync(t => !t.IsDeleted && t.Id == id);
        }

        public async Task<IPagedList<MemberNavigation>> GetAllMembersNavigationPaginated(
            GetPaginatedTeamMembersQuery query)
        {
            return await GetUserTeamIncludeQueryable()
                .Where(uT =>
                    uT.TeamId == query.TeamId && !uT.Team.IsDeleted && !uT.User.IsDeleted && uT.LeaveDate == null)
                .Select(uT => ToMemberNavigation(uT))
                .FromPaginationQueryAsync(query);
        }

      
        private IIncludableQueryable<UserTeam, User> GetUserTeamIncludeQueryable()
        {
            return _context.UserTeams
                .Include(uT => uT.Team)
                .Include(uT => uT.User);
        }


        private static MemberNavigation ToMemberNavigation(UserTeam member) => new MemberNavigation(
            member.UserId,
            member.TeamId,
            member.IsAdmin,
            member.JoinDate,
            member.LeaveDate
        );
    }
}