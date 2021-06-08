using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Write.Contracts;
using Domain.Participations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Write.ParticipationsSessions
{
    public record SetParticipationReadyStateNotification(Guid ParticipationId) : INotification;

    public class SetParticipationReadyStateHandler : INotificationHandler<SetParticipationReadyStateNotification>
    {
        private IParticipationsSessionsRepository _participationsSessionsRepository;

        public SetParticipationReadyStateHandler(IParticipationsSessionsRepository participationsSessionsRepository)
        {
            _participationsSessionsRepository = participationsSessionsRepository;
        }

        public async Task Handle(SetParticipationReadyStateNotification notification,
            CancellationToken cancellationToken)
        {
            var participation =
                await _participationsSessionsRepository.FindByIdAsync(new ParticipationId(notification.ParticipationId));

            if (participation is null)
                throw new NotFoundException(notification.ParticipationId.ToString(), "ParticipationSession");
            participation.SetReadyState();
            await _participationsSessionsRepository.SetAsync(participation);
        }
    }
}