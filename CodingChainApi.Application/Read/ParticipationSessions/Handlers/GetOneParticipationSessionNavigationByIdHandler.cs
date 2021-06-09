using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.ParticipationSessions.Handlers
{
    public record GetOneParticipationSessionNavigationByIdQuery
        (Guid ParticipationId) : IRequest<ParticipationSessionNavigation>;

    public class GetOneParticipationSessionNavigationByIdHandler : IRequestHandler<
        GetOneParticipationSessionNavigationByIdQuery, ParticipationSessionNavigation>
    {
        private readonly IReadParticipationSessionRepository _readParticipationSessionRepository;

        public GetOneParticipationSessionNavigationByIdHandler(
            IReadParticipationSessionRepository readParticipationSessionRepository)
        {
            _readParticipationSessionRepository = readParticipationSessionRepository;
        }

        public async Task<ParticipationSessionNavigation> Handle(GetOneParticipationSessionNavigationByIdQuery request,
            CancellationToken cancellationToken)
        {
            var participation = await _readParticipationSessionRepository.GetOneById(request.ParticipationId);
            if (participation is null)
                throw new NotFoundException(request.ParticipationId.ToString(), "ParticipationSession");
            return participation;
        }
    }
}