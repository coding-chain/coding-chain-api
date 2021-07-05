using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using CodingChainApi.Infrastructure.Hubs;
using Domain.Participations;
using Domain.ParticipationSessions;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CodingChainApi.Infrastructure.Handlers.ParticipationsState
{
    public class
        ParticipationSessionFunctionRemovedHandler : INotificationHandler<
            DomainEventNotification<ParticipationSessionFunctionRemoved>>
    {
        private readonly IHubContext<ParticipationSessionsHub, IParticipationsClient> _hub;

        public ParticipationSessionFunctionRemovedHandler(IHubContext<ParticipationSessionsHub, IParticipationsClient> hub)
        {
            _hub = hub;
        }

        public async Task Handle(DomainEventNotification<ParticipationSessionFunctionRemoved> notification,
            CancellationToken cancellationToken)
        {
            await _hub.Clients.Group(notification.DomainEvent.ParticipationId.ToString())
                .OnFunctionRemoved(new ParticipationFunctionRemovedEvent(notification.DomainEvent.FunctionId.Value));
        }
    }
}