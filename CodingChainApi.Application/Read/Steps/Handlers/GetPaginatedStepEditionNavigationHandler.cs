using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Steps.Handlers
{
    public record GetPaginatedStepNavigationQuery() : PaginationQueryBase,
        IRequest<IPagedList<StepNavigation>>;

    public class GetPaginatedStepEditionNavigationHandler : IRequestHandler<GetPaginatedStepNavigationQuery,
        IPagedList<StepNavigation>>
    {
        private readonly IReadStepRepository _readStepRepository;

        public GetPaginatedStepEditionNavigationHandler(IReadStepRepository readStepRepository)
        {
            _readStepRepository = readStepRepository;
        }

        public Task<IPagedList<StepNavigation>> Handle(GetPaginatedStepNavigationQuery request, CancellationToken cancellationToken)
        {
            return _readStepRepository.GetAllStepNavigationPaginated(request);
        }
    }
}