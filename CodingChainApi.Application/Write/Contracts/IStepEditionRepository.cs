using System.Threading.Tasks;
using Application.Contracts;
using Domain.StepEditions;

namespace Application.Write.Contracts
{
    public interface IStepEditionRepository : IAggregateRepository<StepId, StepEditionAggregate>
    {
        public Task<TestId> GetNextTestIdAsync();
    }
}