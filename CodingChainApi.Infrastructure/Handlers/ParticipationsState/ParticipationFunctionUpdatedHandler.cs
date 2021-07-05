using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using CodingChainApi.Infrastructure.Hubs;
using Domain.Participations;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CodingChainApi.Infrastructure.Handlers.ParticipationsState
{
    public class
        ParticipationFunctionUpdatedHandler : INotificationHandler<
            DomainEventNotification<ParticipationFunctionUpdated>>
    {
        private readonly IHubContext<ParticipationSessionsHub, IParticipationsClient> _hub;

        public ParticipationFunctionUpdatedHandler(IHubContext<ParticipationSessionsHub, IParticipationsClient> hub)
        {
            _hub = hub;
        }

        public async Task Handle(DomainEventNotification<ParticipationFunctionUpdated> notification,
            CancellationToken cancellationToken)
        {
            await _hub.Clients.Group(notification.DomainEvent.ParticipationId.ToString())
                .OnFunctionUpdated(new ParticipationFunctionUpdatedEvent(notification.DomainEvent.FunctionId.Value));
        }
    }
}