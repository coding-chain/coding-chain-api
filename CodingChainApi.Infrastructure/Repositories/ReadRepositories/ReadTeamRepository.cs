using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Teams;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
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
                .Select(t => new TeamNavigation(t.Id, t.Name, t.UserTeams
                    .Where(uT => !uT.User.IsDeleted && uT.LeaveDate == null)
                    .Select(uT => uT.UserId)
                    .ToList()))
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
                team.UserTeams
                    .Where(uT => !uT.User.IsDeleted && uT.LeaveDate == null)
                    .Select(uT => uT.UserId)
                    .ToList()
            );
        }
    }
}