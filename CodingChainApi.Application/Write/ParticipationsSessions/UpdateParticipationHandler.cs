using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Participations;
using Domain.ParticipationStates;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Write.ParticipationsSessions
{
    public class UpdateParticipationHandler : INotificationHandler<DomainEventNotification<ProcessResultUpdated>>
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdateParticipationHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(DomainEventNotification<ProcessResultUpdated> notification,
            CancellationToken cancellationToken)
        {
            var participationId = notification.DomainEvent.ParticipationId.Value;
            using var scope = _serviceProvider.CreateScope();
            var participationsRepository =
                scope.ServiceProvider.GetRequiredService<IParticipationRepository>();
            var participationsSessionsRepository =
                scope.ServiceProvider.GetRequiredService<IReadParticipationSessionRepository>();
            var functionsSessionsRepository =
                scope.ServiceProvider.GetRequiredService<IReadFunctionSessionRepository>();
            var functions =
                await functionsSessionsRepository.GetAllAsync(participationId);
            var participationNav =
                await participationsSessionsRepository.GetOneById(participationId);
            var participation = await participationsRepository.FindByIdAsync(notification.DomainEvent.ParticipationId);
            var functionsEntities = functions.Select(f => new FunctionEntity(
                new FunctionId(f.Id),
                new UserId(f.UserId),
                f.Code,
                f.lastModificationDate,
                f.Order
            )).ToList();
            if (participationNav is null)
            {
                throw new NotFoundException(participationId.ToString(), "ParticipationSession");
            }

            if (participation is null)
            {
                throw new NotFoundException(participationId.ToString(), "Participation");
            }
            participation.Update(participationNav.EndDate, participationNav.CalculatedScore, functionsEntities);
            await participationsRepository.SetAsync(participation);
        }
    }
}