using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Participations.Handlers
{
    public record GetOneParticipationNavigationByIdQuery(Guid ParticipationId) : IRequest<ParticipationNavigation>;
    public class GetOneParticipationNavigationByIdHandler:IRequestHandler<GetOneParticipationNavigationByIdQuery, ParticipationNavigation>
    {
        private readonly IReadParticipationRepository _readParticipationRepository;

        public GetOneParticipationNavigationByIdHandler(IReadParticipationRepository readParticipationRepository)
        {
            _readParticipationRepository = readParticipationRepository;
        }

        public async Task<ParticipationNavigation> Handle(GetOneParticipationNavigationByIdQuery request, CancellationToken cancellationToken)
        {
            var participation = await _readParticipationRepository.GetOneParticipationNavigationById(request.ParticipationId);
            if (participation is null)
                throw new NotFoundException(request.ParticipationId.ToString(), nameof(ParticipationNavigation));
            return participation;
        }
    }
}