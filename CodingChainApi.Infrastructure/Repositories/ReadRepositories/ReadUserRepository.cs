using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Tournaments.Handlers;
using Application.Read.Users;
using Application.Read.Users.Handlers;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadUserRepository : IReadUserRepository
    {
        private readonly CodingChainContext _context;

        public ReadUserRepository(CodingChainContext context)
        {
            _context = context;
        }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return (await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted)) is not null;
        }

        public async Task<bool> UserExistsByIdAsync(Guid id)
        {
            return (await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted)) is not null;
        }

        public async Task<Guid?> FindUserIdByEmail(string email)
        {
            return (await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted))?.Id;
        }

        private static Expression<Func<User, bool>> ToPredicate(GetPaginatedPublicUsersQuery query) =>
            user =>
                !user.IsDeleted
                && (query.UsernameFilter == null || user.Username.Contains(query.UsernameFilter))
                || (query.EmailFilter == null || user.Email.Contains(query.EmailFilter));


        public async Task<PublicUser?> FindPublicUserById(Guid id)
        {
            var user = await GetPublicUserIncludeQueryable().FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user is null) return null;
            return ToPublicUser(user);
        }


        public async Task<IPagedList<PublicUser>> FindPaginatedPublicUsers(GetPaginatedPublicUsersQuery query)
        {
            return await GetPublicUserIncludeQueryable()
                .Where( ToPredicate( query))
                .Select(u => ToPublicUser(u))
                .FromPaginationQueryAsync(query);
        }

        private IIncludableQueryable<User, Team> GetPublicUserIncludeQueryable()
        {
            return _context.Users
                .Include(u => u.Rights)
                .Include(u => u.UserTeams)
                .ThenInclude(uT => uT.Team);
        }

        private static PublicUser ToPublicUser(User user) => new PublicUser(
            user.Id,
            user.Username,
            user.Email,
            user.Rights.Select(r => r.Id).ToList(),
            user.UserTeams.Where(uT => uT.LeaveDate is null && !uT.Team.IsDeleted)
                .Select(uT => uT.TeamId)
                .ToList()
        );
    }
}