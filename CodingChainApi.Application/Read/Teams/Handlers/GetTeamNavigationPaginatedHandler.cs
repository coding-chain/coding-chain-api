using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Teams.Handlers
{
    public record GetTeamNavigationPaginatedQuery: PaginationQueryBase, IRequest<IPagedList<TeamNavigation>>
    {
        public string? NameFilter { get; set; }
        public OrderEnum? NameOrder { get; set; }
        
        public Guid? TournamentFilter { get; set; }
        public Guid? MemberIdFilter { get; set; }
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