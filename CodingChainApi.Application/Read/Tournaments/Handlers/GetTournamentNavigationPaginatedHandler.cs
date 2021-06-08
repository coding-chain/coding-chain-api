using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Tournaments.Handlers
{
    public record GetTournamentNavigationPaginatedQuery :
        PaginationQueryBase,
        IRequest<IPagedList<TournamentNavigation>>
    {
        public Guid? LanguageIdFilter { get; set; }
        public string? NameFilter { get; set; }

        public OrderEnum? NameOrder { get; set; }

        public Guid? ParticipantIdFilter { get; set; }

        public bool? IsPublishedFilter { get; set; }

        public Guid? TeamId { get; set; }
    }

    public class GetTournamentNavigationPaginatedHandler : IRequestHandler<GetTournamentNavigationPaginatedQuery,
        IPagedList<TournamentNavigation>>
    {
        private readonly IReadTournamentRepository _readTournamentRepository;

        public GetTournamentNavigationPaginatedHandler(IReadTournamentRepository readTournamentRepository)
        {
            _readTournamentRepository = readTournamentRepository;
        }

        public async Task<IPagedList<TournamentNavigation>> Handle(GetTournamentNavigationPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            return await _readTournamentRepository.GetAllTournamentNavigationPaginated(request);
        }
    }
}