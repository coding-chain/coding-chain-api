using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Read.Contracts;
using CodingChainApi.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadUserRepository: IReadUserRepository
    {

        private readonly CodingChainContext _context;

        public ReadUserRepository(CodingChainContext context)
        {
            _context = context;
        }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return (await _context.Users.FirstOrDefaultAsync(u => u.Email == email)) is not null;
        }

        public async Task<Guid?> FindUserIdByEmail(string email)
        {
            return (await _context.Users.FirstOrDefaultAsync(u => u.Email == email))?.Id;
        }
    }
}