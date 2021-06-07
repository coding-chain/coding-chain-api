using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Write.Contracts;
using CodingChainApi.WebApi.Client.Contracts;
using Domain.Exceptions;
using Domain.Participations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Write.ParticipationsSessions
{
    public record SetParticipationReadyStateNotification(Guid ParticipationId) : INotification;
    public class SetParticipationReadyStateHandler: INotificationHandler<SetParticipationReadyStateNotification>
    {
        private readonly IServiceProvider _serviceProvider;

        public SetParticipationReadyStateHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(SetParticipationReadyStateNotification notification, CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            var participationsSessionsRepository =  scope.ServiceProvider.GetRequiredService<IParticipationsSessionsRepository>();
            var participation = await participationsSessionsRepository.FindByIdAsync(new ParticipationId(notification.ParticipationId));

            if (participation is null)
                throw new NotFoundException(notification.ParticipationId.ToString(), "ParticipationSession");
            participation.SetReadyState();
            await participationsSessionsRepository.SetAsync(participation);
        }
    }
}