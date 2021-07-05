using System;
using System.Threading.Tasks;
using Application.Read.ParticipationSessions;

namespace Application.Read.Contracts
{
    public interface IReadParticipationSessionRepository
    {
        public Task<bool> ExistsById(Guid id);
        public Task<ParticipationSessionNavigation?> GetOneById(Guid id);
    }
}