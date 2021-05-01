using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Contracts;
using Domain.Users;

namespace Application.Write.Contracts
{
    public interface IUserRepository :IAggregateRepository<UserId, User>
    {
        Task<IList<User>> GetAllAsync();
    }
}