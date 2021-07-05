using Application.Contracts;
using Domain.Teams;

namespace Application.Write.Contracts
{
    public interface ITeamRepository : IAggregateRepository<TeamId, TeamAggregate>
    {
    }
}