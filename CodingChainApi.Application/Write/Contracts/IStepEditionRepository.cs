using System.Threading.Tasks;
using Application.Contracts;
using Domain.StepEditions;
using Domain.Steps;

namespace Application.Write.Contracts
{
    public interface IStepEditionRepository: IAggregateRepository<StepId,StepEditionAggregate>
    {
        public Task<TestId> GetNextTestIdAsync();
    }
}