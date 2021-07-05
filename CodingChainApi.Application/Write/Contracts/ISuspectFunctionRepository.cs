using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Contracts;
using Domain.CodeAnalysis;
using Domain.Participations;

namespace Application.Write.Contracts
{
    public interface ISuspectFunctionRepository : IAggregateRepository<FunctionId, SuspectFunctionAggregate>
    {
        public Task<IList<SuspectFunctionAggregate>> GetAllAsync();
    }
}