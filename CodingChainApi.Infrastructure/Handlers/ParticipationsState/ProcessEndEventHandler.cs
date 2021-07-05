using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using CodingChainApi.Infrastructure.Hubs;
using Domain.ParticipationSessions;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CodingChainApi.Infrastructure.Handlers.ParticipationsState
{
    public class ProcessEndEventHandler : INotificationHandler<DomainEventNotification<ProcessResultUpdated>>
    {
        private readonly IHubContext<ParticipationSessionsHub, IParticipationsClient> _hub;

        public ProcessEndEventHandler(IHubContext<ParticipationSessionsHub, IParticipationsClient> hub)
        {
            _hub = hub;
        }

        public async Task Handle(DomainEventNotification<ProcessResultUpdated> notification,
            CancellationToken cancellationToken)
        {
            await _hub.Clients
                .Group(notification.DomainEvent.ParticipationId.ToString())
                .OnProcessEnd(new ParticipationProcessEndEvent(
                    notification.DomainEvent.ParticipationId.Value
                ));
        }
    }
}