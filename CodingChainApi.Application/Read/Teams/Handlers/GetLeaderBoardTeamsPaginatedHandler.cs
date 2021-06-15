using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Teams.Handlers
{
    public record GetLeaderBoardTeamsPaginatedQuery: PaginationQueryBase, 
        IRequest<IPagedList<LeaderBoardTeamNavigation>>
    {
        public Guid? TournamentIdFilter { get; set; }
        public OrderEnum? ScoreOrder { get; set; }
        public bool? HasFinishedFilter { get; set; }
    }
    public class GetLeaderBoardTeamsPaginatedHandler: IRequestHandler<GetLeaderBoardTeamsPaginatedQuery, IPagedList<LeaderBoardTeamNavigation>>
    {
        private readonly IReadTeamRepository _readTeamRepository;

        public GetLeaderBoardTeamsPaginatedHandler(IReadTeamRepository readTeamRepository)
        {
            _readTeamRepository = readTeamRepository;
        }

        public Task<IPagedList<LeaderBoardTeamNavigation>> Handle(GetLeaderBoardTeamsPaginatedQuery request, CancellationToken cancellationToken)
        {
            return _readTeamRepository.GetAllLeaderBoardTeamNavigationPaginated(request);
        }
    }
}