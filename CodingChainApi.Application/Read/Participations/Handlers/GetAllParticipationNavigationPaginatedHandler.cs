using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Participations.Handlers
{
    public record GetAllParticipationNavigationPaginatedQuery : PaginationQueryBase,
        IRequest<IPagedList<ParticipationNavigation>>
    {
        public Guid? TeamIdFilter { get; set; }
        public Guid? TournamentIdFilter { get; set; }
        public Guid? StepIdFilter { get; set; }
        
        public bool? EndTournamentStepFilter { get; set; }
        public OrderEnum? EndDateOrder { get; set; }
    }


    public class GetAllParticipationNavigationPaginatedHandler : IRequestHandler<
        GetAllParticipationNavigationPaginatedQuery, IPagedList<ParticipationNavigation>>
    {
        private readonly IReadParticipationRepository _readParticipationRepository;

        public GetAllParticipationNavigationPaginatedHandler(IReadParticipationRepository readParticipationRepository)
        {
            _readParticipationRepository = readParticipationRepository;
        }

        public Task<IPagedList<ParticipationNavigation>> Handle(GetAllParticipationNavigationPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            return _readParticipationRepository.GetAllParticipationNavigationPaginated(request);
        }
    }

   
}