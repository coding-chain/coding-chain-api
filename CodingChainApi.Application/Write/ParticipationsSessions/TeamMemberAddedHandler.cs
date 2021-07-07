using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Participations;
using Domain.Teams;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Write.ParticipationsSessions
{
    public class TeamMemberAddedHandler : INotificationHandler<DomainEventNotification<TeamMemberAdded>>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TeamMemberAddedHandler> _logger;

        public TeamMemberAddedHandler(IServiceProvider serviceProvider, ILogger<TeamMemberAddedHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(DomainEventNotification<TeamMemberAdded> notification,
            CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var participationRepository = scope.ServiceProvider.GetRequiredService<IParticipationsSessionsRepository>();
            var readParticipationRepository = scope.ServiceProvider.GetRequiredService<IReadParticipationRepository>();
            var participations =
                await readParticipationRepository.GetAllParticipationsByTeamId(notification.DomainEvent.TeamId.Value);

            foreach (var participationNavigation in participations)
            {
                try
                {
                    var participationSession =
                        await participationRepository.FindByIdAsync(new ParticipationId(participationNavigation.Id));
                    if (participationSession is not null)
                    {
                        participationSession.AddTeamMember(notification.DomainEvent.MemberId);
                        await participationRepository.SetAsync(participationSession);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }
        }
    }
}