using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Users;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PublicUser?> FindPublicUserById(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user is null) return null;
            return new PublicUser(user.Id, user.Username, user.Email);
        }

        public async Task<IPagedList<PublicUser>> FindPaginatedPublicUsers(PaginationQueryBase query)
        {
            return await _context.Users
                .Select(u => new PublicUser(u.Id, u.Username, u.Email))
                .FromPaginationQueryAsync(query);
        }
    }
}