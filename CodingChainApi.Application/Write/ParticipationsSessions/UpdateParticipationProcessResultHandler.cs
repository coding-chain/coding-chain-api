using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Write.Contracts;
using Domain.Participations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Write.ParticipationsSessions
{
    public record UpdateParticipationProcessNotification
        (Guid ParticipationId, string? Output, string? Error) : INotification;

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
            var participation =
                await participationsSessionsRepository.FindByIdAsync(
                    new ParticipationId(notification.ParticipationId));
            if (participation is null)
            {
                throw new NotFoundException(notification.ParticipationId.ToString(), "Participation");
            }

            participation.SetProcessResult(notification.Error, notification.Output);
            await participationsSessionsRepository.SetAsync(participation);
        }
    }
}