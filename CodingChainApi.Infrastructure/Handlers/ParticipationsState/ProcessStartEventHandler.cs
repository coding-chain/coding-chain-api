using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using CodingChainApi.Infrastructure.Hubs;
using Domain.ParticipationStates;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CodingChainApi.Infrastructure.Handlers.ParticipationsState
{
    public class ProcessStartEventHandler : INotificationHandler<DomainEventNotification<ProcessStarted>>
    {
        private readonly IHubContext<ParticipationSessionsHub, IParticipationsClient> _hub;

        public ProcessStartEventHandler(IHubContext<ParticipationSessionsHub, IParticipationsClient> hub)
        {
            _hub = hub;
        }


        public async Task Handle(DomainEventNotification<ProcessStarted> notification,
            CancellationToken cancellationToken)
        {
            await _hub.Clients
                .Group(notification.DomainEvent.ParticipationId.ToString())
                .OnProcessStart(new ParticipationProcessStartEvent(
                    notification.DomainEvent.ParticipationId.Value
                ));
        }
    }
}