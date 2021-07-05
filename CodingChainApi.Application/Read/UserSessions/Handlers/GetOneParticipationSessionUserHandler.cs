using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.UserSessions.Handlers
{
    public record GetOneParticipationSessionUserQuery
        (Guid ParticipationId, Guid UserId) : IRequest<UserSessionNavigation>;

    public class GetOneParticipationSessionUserHandler : IRequestHandler<
        GetOneParticipationSessionUserQuery, UserSessionNavigation>
    {
        private readonly IReadParticipationSessionRepository _readParticipationSessionRepository;
        private readonly IReadUserSessionRepository _readUserSessionRepository;

        public GetOneParticipationSessionUserHandler(IReadUserSessionRepository readUserSessionRepository,
            IReadParticipationSessionRepository readParticipationSessionRepository)
        {
            _readUserSessionRepository = readUserSessionRepository;
            _readParticipationSessionRepository = readParticipationSessionRepository;
        }

        public async Task<UserSessionNavigation> Handle(GetOneParticipationSessionUserQuery request,
            CancellationToken cancellationToken)
        {
            if (!await _readParticipationSessionRepository.ExistsById(request.ParticipationId))
                throw new NotFoundException(request.ParticipationId.ToString(), "ParticipationSession");

            var function = await _readUserSessionRepository
                .GetOneUserNavigationByIdAsync(request.ParticipationId, request.UserId);
            if (function is null)
                throw new NotFoundException(
                    $"ParticipationId : {request.ParticipationId}, UserId: {request.UserId}",
                    "UserSession");

            return function;
        }
    }
}