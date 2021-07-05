using Application.Contracts;
using Domain.Tournaments;

namespace Application.Write.Contracts
{
    public interface ITournamentRepository : IAggregateRepository<TournamentId, TournamentAggregate>
    {
    }
}