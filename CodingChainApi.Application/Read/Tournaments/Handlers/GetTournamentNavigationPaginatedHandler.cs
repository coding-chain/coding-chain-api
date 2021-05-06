using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Tournaments.Handlers
{
    public record GetTournamentNavigationPaginatedQuery() : PaginationQueryBase,
        IRequest<IPagedList<TournamentNavigation>>;
    public class GetTournamentNavigationPaginatedHandler: IRequestHandler<GetTournamentNavigationPaginatedQuery, IPagedList<TournamentNavigation>>
    {
        private readonly IReadTournamentRepository _readTournamentRepository;

        public GetTournamentNavigationPaginatedHandler(IReadTournamentRepository readTournamentRepository)
        {
            _readTournamentRepository = readTournamentRepository;
        }

        public async Task<IPagedList<TournamentNavigation>> Handle(GetTournamentNavigationPaginatedQuery request, CancellationToken cancellationToken)
        {
            return await _readTournamentRepository.GetAllTournamentNavigationPaginated(request);
        }
    }
}