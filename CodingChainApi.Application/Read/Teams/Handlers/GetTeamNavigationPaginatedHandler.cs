using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Languages;
using MediatR;

namespace Application.Read.Teams.Handlers
{
    public record GetTeamNavigationPaginatedQuery: PaginationQueryBase, IRequest<IPagedList<TeamNavigation>>
    {
    }

    public class GetTeamNavigationPaginatedHandler: IRequestHandler<GetTeamNavigationPaginatedQuery, IPagedList<TeamNavigation>>
    {
        private readonly IReadTeamRepository _readTeamRepository;

        public GetTeamNavigationPaginatedHandler(IReadTeamRepository readTeamRepository)
        {
            _readTeamRepository = readTeamRepository;
        }

        public Task<IPagedList<TeamNavigation>> Handle(GetTeamNavigationPaginatedQuery request, CancellationToken cancellationToken)
        {
            return _readTeamRepository.GetAllTeamNavigationPaginated(request);
        }
    }
}