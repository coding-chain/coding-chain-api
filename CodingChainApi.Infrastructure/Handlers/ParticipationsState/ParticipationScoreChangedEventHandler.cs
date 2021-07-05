using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using CodingChainApi.Infrastructure.Hubs;
using Domain.ParticipationSessions;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CodingChainApi.Infrastructure.Handlers.ParticipationsState
{
    public class
        ParticipationScoreChangedEventHandler : INotificationHandler<
            DomainEventNotification<ParticipationSessionScoreChanged>>
    {
        private readonly IHubContext<ParticipationSessionsHub, IParticipationsClient> _hub;

        public ParticipationScoreChangedEventHandler(IHubContext<ParticipationSessionsHub, IParticipationsClient> hub)
        {
            _hub = hub;
        }

        public async Task Handle(DomainEventNotification<ParticipationSessionScoreChanged> notification,
            CancellationToken cancellationToken)
        {
            await _hub.Clients.Group(notification.DomainEvent.ParticipationId.ToString())
                .OnScoreChanged(new ParticipationScoreChangedEvent(notification.DomainEvent.ParticipationId.Value));
        }
    }
}