using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Contracts;
using Domain.CodeAnalysis;
using Domain.Participations;

namespace Application.Write.Contracts
{
    public interface IPlagiarizedFunctionRepository : IAggregateRepository<FunctionId, SuspectFunctionAggregate>
    {
        public Task<IList<SuspectFunctionAggregate>> GetAllAsync();
    }
}