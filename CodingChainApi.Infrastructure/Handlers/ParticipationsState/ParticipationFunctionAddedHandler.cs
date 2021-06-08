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
        ParticipationFunctionAddedHandler : INotificationHandler<DomainEventNotification<ParticipationFunctionAdded>>
    {
        private readonly IHubContext<ParticipationSessionsHub, IParticipationsClient> _hub;

        public ParticipationFunctionAddedHandler(IHubContext<ParticipationSessionsHub, IParticipationsClient> hub)
        {
            _hub = hub;
        }

        public async Task Handle(DomainEventNotification<ParticipationFunctionAdded> notification,
            CancellationToken cancellationToken)
        {
            await _hub.Clients.Group(notification.DomainEvent.ParticipationId.ToString())
                .OnFunctionAdded(new ParticipationFunctionAddedEvent(notification.DomainEvent.FunctionId.Value));
        }
    }
}