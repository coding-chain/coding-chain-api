using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using CodingChainApi.Infrastructure.Hubs;
using Domain.ParticipationStates;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CodingChainApi.Infrastructure.Handlers.ParticipationsState
{
    public class ConnectedUserUpdatedHandler : INotificationHandler<DomainEventNotification<ConnectedUserUpdated>>
    {
        private readonly IHubContext<ParticipationSessionsHub, IParticipationsClient> _hub;

        public ConnectedUserUpdatedHandler(IHubContext<ParticipationSessionsHub, IParticipationsClient> hub)
        {
            _hub = hub;
        }

        public async Task Handle(DomainEventNotification<ConnectedUserUpdated> notification,
            CancellationToken cancellationToken)
        {
            await _hub.Clients.Group(notification.DomainEvent.ParticipationId.ToString())
                .OnUpdatedConnectedUser(new(notification.DomainEvent.UserId.Value));
        }
    }
}