using System;
using System.Threading.Tasks;
using Domain.Users;

namespace Application.Read.Contracts
{
    public interface IReadUserRepository
    {
        public Task<bool> UserExistsByEmailAsync(string email);
        public Task<Guid?> FindUserIdByEmail(string email);
    }
}