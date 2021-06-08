using Application.Contracts;
using Domain.Participations;

namespace Application.Write.Contracts
{
    public interface IParticipationRepository : IAggregateRepository<ParticipationId, ParticipationAggregate>
    {
    }
}