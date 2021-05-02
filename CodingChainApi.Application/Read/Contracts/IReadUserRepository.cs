using System;
using System.Threading.Tasks;
using Application.Read.Users;
using Domain.Users;

namespace Application.Read.Contracts
{
    public interface IReadUserRepository
    {
        public Task<bool> UserExistsByEmailAsync(string email);
        public Task<Guid?> FindUserIdByEmail(string email);

        public Task<ConnectedUser?> FindConnectedUserById(Guid id);
    }
}