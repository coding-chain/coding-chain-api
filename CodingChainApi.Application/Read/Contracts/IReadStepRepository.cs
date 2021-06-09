using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Steps;
using Application.Read.Steps.Handlers;

namespace Application.Read.Contracts
{
    public interface IReadStepRepository
    {
        public Task<bool> StepExistsById(Guid stepId);
        public Task<bool> StepExistsByIds(Guid[] stepIds);

        public Task<IPagedList<StepNavigation>> GetAllStepNavigationPaginated(
            GetPaginatedStepNavigationQuery paginationQuery);

        public Task<StepNavigation?> GetOneStepNavigationById(Guid id);
    }
}