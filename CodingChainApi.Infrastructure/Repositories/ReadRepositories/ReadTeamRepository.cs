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

        public async Task<IPagedList<TeamNavigation>> GetAllTeamNavigationPaginated(PaginationQueryBase paginationQuery)
        {
            return await _context.Teams
                .Include(t => t.UserTeams)
                .ThenInclude(uT => uT.User)
                .Where(t => !t.IsDeleted)
                .Select(t => new TeamNavigation(t.Id, t.Name, ToMemberNavigations(t)))
                .FromPaginationQueryAsync(paginationQuery);
        }

        public async Task<TeamNavigation?> GetOneTeamNavigationByIdAsync(Guid id)
        {
            var team = await _context.Teams
                .Include(t => t.UserTeams)
                .ThenInclude(uT => uT.User)
                .FirstOrDefaultAsync(t => !t.IsDeleted);
            if (team is null) return null;
            return new TeamNavigation(
                team.Id,
                team.Name,
                ToMemberNavigations(team)
            );
        }

        private static List<MemberNavigation> ToMemberNavigations(Team team)
        {
            return team.UserTeams
                .Where(uT => !uT.User.IsDeleted && uT.LeaveDate == null)
                .Select(uT => new MemberNavigation(uT.UserId, uT.IsAdmin, uT.JoinDate, uT.LeaveDate))
                .ToList();
        }
    }
}