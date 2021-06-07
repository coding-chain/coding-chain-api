using Application.Contracts;
using Domain.CodeAnalysis;
using Domain.Participations;

namespace Application.Write.Contracts
{
    public interface IPlagiarizedFunctionRepository : IAggregateRepository<FunctionId, PlagiarizedFunctionAggregate>
    {
    }
}