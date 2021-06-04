using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.Participations;
using MediatR;

namespace Application.Write.ParticipationsSessions
{
    [Authenticated]
    public record DisconnectUserFromParticipationCommand(Guid ParticipationId) : IRequest<int>;

    public class DisconnectUserFromParticipationHandler : IRequestHandler<DisconnectUserFromParticipationCommand, int>
    {
        private readonly IParticipationsSessionsRepository _participationsSessionsRepository;
        private readonly ICurrentUserService _currentUserService;
        public DisconnectUserFromParticipationHandler(IParticipationRepository participationRepository,
            IParticipationsSessionsRepository participationsSessionsRepository, ICurrentUserService currentUserService)
        {
            _participationsSessionsRepository = participationsSessionsRepository;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(DisconnectUserFromParticipationCommand request, CancellationToken cancellationToken)
        {
            var participation =
                await _participationsSessionsRepository.FindByIdAsync(new ParticipationId(request.ParticipationId));
            if (participation is null)
            {
                throw new NotFoundException(request.ParticipationId.ToString(), "Participation");
            }
            var connectionCount = participation.RemoveConnectedUser(_currentUserService.UserId);
            await _participationsSessionsRepository.SetAsync(participation);
            return connectionCount;
        }
    }
}