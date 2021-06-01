using System.Linq;
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
        ParticipationFunctionsReorderedHandler : INotificationHandler<
            DomainEventNotification<ParticipationFunctionsReordered>>
    {
        private readonly IHubContext<ParticipationSessionsHub, IParticipationsClient> _hub;

        public ParticipationFunctionsReorderedHandler(IHubContext<ParticipationSessionsHub, IParticipationsClient> hub)
        {
            _hub = hub;
        }

        public async Task Handle(DomainEventNotification<ParticipationFunctionsReordered> notification,
            CancellationToken cancellationToken)
        {
            await _hub.Clients.Group(notification.DomainEvent.ParticipationId.ToString()).OnFunctionsReordered(
                new ParticipationFunctionsReorderedEvent(notification.DomainEvent.FunctionIds.Select(f => f.Value)
                    .ToList()));
        }
    }
}