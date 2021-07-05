using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.CodeAnalysis;
using Domain.Participations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Write.ParticipationsSessions
{
    public class ParticipationSuspectFunctionRemovedHandler : INotificationHandler<
        DomainEventNotification<ParticipationSuspectFunctionRemoved>>
    {
        private readonly IServiceProvider _serviceProvider;


        public ParticipationSuspectFunctionRemovedHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(DomainEventNotification<ParticipationSuspectFunctionRemoved> notification,
            CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var participationsRepository =
                scope.ServiceProvider.GetRequiredService<IParticipationsSessionsRepository>();
            var participation = await participationsRepository.FindByIdAsync(notification.DomainEvent.ParticipationId);
            if (participation is null)
                throw new NotFoundException(notification.DomainEvent.ParticipationId.ToString(),
                    "ParticipationSession");
            participation.RemoveSuspectFunction(notification.DomainEvent.FunctionId);
            await participationsRepository.SetAsync(participation);
        }
    }
}