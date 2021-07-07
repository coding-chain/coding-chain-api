using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using CodingChainApi.Infrastructure.Hubs;
using Domain.ParticipationSessions;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CodingChainApi.Infrastructure.Handlers.ParticipationsState
{
    public class ParticipationUserRemovedHandler: INotificationHandler<DomainEventNotification<ParticipationUserRemoved>>
    {
        private readonly IHubContext<ParticipationSessionsHub, IParticipationsClient> _hub;

        public ParticipationUserRemovedHandler(IHubContext<ParticipationSessionsHub, IParticipationsClient> hub)
        {
            _hub = hub;
        }

        public async Task Handle(DomainEventNotification<ParticipationUserRemoved> notification,
            CancellationToken cancellationToken)
        {
            await _hub.Clients
                .Group(notification.DomainEvent.ParticipationId.ToString())
                .OnDisconnectedUser(new (
                    notification.DomainEvent.UserId.Value
                ));
        }
    }
}