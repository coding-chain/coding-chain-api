using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Teams;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadTeamRepository : IReadTeamRepository
    {
        private readonly CodingChainContext _context;

        public ReadTeamRepository(CodingChainContext context)
        {
            _context = context;
        }

        private static TeamNavigation ToTeamNavigation(Team team) => new TeamNavigation(
            team.Id,
            team.Name,
            team.UserTeams
                .Where(uT => uT.LeaveDate == null && !uT.User.IsDeleted)
                .Select(uT => uT.Id).ToList()
        );

        public async Task<IPagedList<TeamNavigation>> GetAllTeamNavigationPaginated(PaginationQueryBase paginationQuery)
        {
            return await _context.Teams
                .Include(t => t.UserTeams)
                .ThenInclude(uT => uT.User)
                .Where(t => !t.IsDeleted)
                .Select(t => ToTeamNavigation(t))
                .FromPaginationQueryAsync(paginationQuery);
        }

        public async Task<TeamNavigation?> GetOneTeamNavigationByIdAsync(Guid id)
        {
            var team = await _context.Teams
                .Include(t => t.UserTeams)
                .ThenInclude(uT => uT.User)
                .FirstOrDefaultAsync(t => !t.IsDeleted);
            if (team is null) return null;
            return ToTeamNavigation(team);
        }

        public async Task<MemberNavigation?> GetOneMemberNavigationByIdAsync(Guid teamId, Guid userId)
        {
            var member = await _context.UserTeams
                .Include(uT => uT.Team)
                .Include(uT => uT.User)
                .FirstOrDefaultAsync(uT =>
                    uT.TeamId == teamId && uT.UserId == userId && !uT.Team.IsDeleted && !uT.User.IsDeleted &&
                    uT.LeaveDate == null);
            return member is null ? null : ToMemberNavigation(member);
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