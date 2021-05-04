using System.Threading.Tasks;
using Domain.Users;

namespace Application.Contracts.IService
{
    public interface ITokenService
    {
        public Task<string> GenerateUserTokenAsync(UserAggregate user);
    }
}