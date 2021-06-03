using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.Participations;
using Domain.StepEditions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Write.ParticipationsSessions
{
    public record UpdateParticipationProcessNotification
        (Guid ParticipationId, string? Output, string? Error, IList<Guid> TestsPassedIds) : INotification;

    public class UpdateParticipationProcessResultHandler : INotificationHandler<UpdateParticipationProcessNotification>
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdateParticipationProcessResultHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(UpdateParticipationProcessNotification notification,
            CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var participationsSessionsRepository =
                scope.ServiceProvider.GetRequiredService<IParticipationsSessionsRepository>();
            var timeService =
                scope.ServiceProvider.GetRequiredService<ITimeService>();
            var participation =
                await participationsSessionsRepository.FindByIdAsync(
                    new ParticipationId(notification.ParticipationId));
            if (participation is null)
            {
                throw new NotFoundException(notification.ParticipationId.ToString(), "Participation");
            }

            var testsPassedIds = notification.TestsPassedIds.Select(id => new TestId(id)).ToList();
            participation.SetProcessResult(notification.Error, notification.Output, testsPassedIds, timeService.Now());
            await participationsSessionsRepository.SetAsync(participation);
        }
        
    }
}