using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Tournaments.Handlers
{
    public record GetPaginatedTournamentStepNavigationQuery : PaginationQueryBase,
        IRequest<IPagedList<TournamentStepNavigation>>
    {
        public Guid TournamentId { get; set; }
    }

    public class GetPaginatedTournamentStepNavigationHandler : IRequestHandler<GetPaginatedTournamentStepNavigationQuery
        , IPagedList<TournamentStepNavigation>>
    {
        private readonly IReadTournamentRepository _readTournamentRepository;

        public GetPaginatedTournamentStepNavigationHandler(IReadTournamentRepository readTournamentRepository)
        {
            _readTournamentRepository = readTournamentRepository;
        }

        public Task<IPagedList<TournamentStepNavigation>> Handle(GetPaginatedTournamentStepNavigationQuery request,
            CancellationToken cancellationToken)
        {
            return _readTournamentRepository.GetAllTournamentStepNavigationPaginated(request);
        }
    }
}