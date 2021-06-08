using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using CodingChainApi.Infrastructure.Hubs;
using Domain.ParticipationSessions;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CodingChainApi.Infrastructure.Handlers.ParticipationsState
{
    public class ParticipationReadyEventHandler : INotificationHandler<DomainEventNotification<ParticipationReady>>
    {
        private readonly IHubContext<ParticipationSessionsHub, IParticipationsClient> _hub;

        public ParticipationReadyEventHandler(IHubContext<ParticipationSessionsHub, IParticipationsClient> hub)
        {
            _hub = hub;
        }

        public async Task Handle(DomainEventNotification<ParticipationReady> notification,
            CancellationToken cancellationToken)
        {
            await _hub.Clients
                .Group(notification.DomainEvent.ParticipationId.ToString())
                .OnReady(new ParticipationReadyEvent(
                    notification.DomainEvent.ParticipationId.Value
                ));
        }
    }
}