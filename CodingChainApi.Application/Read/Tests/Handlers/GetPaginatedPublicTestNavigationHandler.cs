using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Tests.Handlers
{
    public record GetPaginatedPublicTestNavigationQuery : PaginationQueryBase, IRequest<IPagedList<PublicTestNavigation>>
    {
        public Guid? StepId { get; set; }
    }
    public class GetPaginatedPublicTestNavigationHandler : IRequestHandler<GetPaginatedPublicTestNavigationQuery, IPagedList<PublicTestNavigation>>
    {
        private IReadTestRepository _readTestRepository;

        public GetPaginatedPublicTestNavigationHandler(IReadTestRepository readTestRepository, IReadStepRepository readStepRepository)
        {
            _readTestRepository = readTestRepository;
        }

        public async Task<IPagedList<PublicTestNavigation>> Handle(GetPaginatedPublicTestNavigationQuery request, CancellationToken cancellationToken)
        {
            return await _readTestRepository.GetPaginatedPublicTestNavigation(request);
        }
    }
}