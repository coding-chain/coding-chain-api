using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Tests.Handlers
{
    public record GetPaginatedTestNavigationQuery() : PaginationQueryBase, IRequest<IPagedList<TestNavigation>>;
    public class GetPaginatedTestNavigationHandler : IRequestHandler<GetPaginatedTestNavigationQuery, IPagedList<TestNavigation>>
    {
        private IReadTestRepository _readTestRepository;

        public GetPaginatedTestNavigationHandler(IReadTestRepository readTestRepository)
        {
            _readTestRepository = readTestRepository;
        }

        public Task<IPagedList<TestNavigation>> Handle(GetPaginatedTestNavigationQuery request, CancellationToken cancellationToken)
        {
            return _readTestRepository.GetPaginatedTestNavigation(request);
        }
    }
}