using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Rights.Handlers
{
    public record GetPaginatedRightNavigationsQuery() : PaginationQueryBase, IRequest<IPagedList<RightNavigation>>;
    public class GetPaginatedRightNavigationsHandler: IRequestHandler<GetPaginatedRightNavigationsQuery, IPagedList<RightNavigation>>
    {
        private readonly IReadRightRepository _readRightRepository;

        public GetPaginatedRightNavigationsHandler(IReadRightRepository readRightRepository)
        {
            _readRightRepository = readRightRepository;
        }

        public Task<IPagedList<RightNavigation>> Handle(GetPaginatedRightNavigationsQuery request, CancellationToken cancellationToken)
        {
            return _readRightRepository.GetAllRightNavigationPaginated(request);
        }
    }
}