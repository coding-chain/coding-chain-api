using System;
using System.Threading.Tasks;

namespace Application.Contracts.IService
{
    public interface ITokenService
    {
        public Task<string> GenerateUserTokenAsync(Guid userId);
        public Task<string> GenerateUserParticipationTokenAsync(Guid userId, Guid participationId);
    }
}