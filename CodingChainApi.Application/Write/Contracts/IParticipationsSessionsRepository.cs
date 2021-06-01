using System.Threading.Tasks;
using Application.Contracts;
using Domain.Participations;
using Domain.ParticipationStates;

namespace Application.Write.Contracts
{
    public interface IParticipationsSessionsRepository: IAggregateRepository<ParticipationId, ParticipationSessionAggregate>
    {
        Task<FunctionId> GetNextFunctionId();
    }
}