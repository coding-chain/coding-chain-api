using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using Application.Read.Functions;
using Application.Read.Steps;
using Application.Read.Tournaments;
using Application.Write.Contracts;
using Domain.CodeAnalysis;
using Domain.Exceptions;
using Domain.Participations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Write.Participations
{
    public class
        SuspectFunctionPlagiarismConfirmedHandler : INotificationHandler<
            DomainEventNotification<SuspectFunctionPlagiarismConfirmed>>
    {
        private readonly IServiceProvider _serviceProvider;


        public SuspectFunctionPlagiarismConfirmedHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(DomainEventNotification<SuspectFunctionPlagiarismConfirmed> notification,
            CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var readFuncRepository = scope.ServiceProvider.GetRequiredService<IReadFunctionRepository>();

            var func = await readFuncRepository.GetById(notification.DomainEvent.Id.Value, false);
            if (func is null)
                throw new NotFoundException(notification.DomainEvent.Id.ToString(), "SuspectFunction");
            var participationsRepository =
                scope.ServiceProvider.GetRequiredService<IParticipationRepository>();
            var participation = await participationsRepository.FindByIdAsync(new ParticipationId(func.ParticipationId));
            if (participation is null)
                throw new NotFoundException(func.ParticipationId.ToString(), "Participation");
            participation.RemoveSuspectFunction(new FunctionId(func.Id));
            await participationsRepository.SetAsync(participation);
            
        }

      
    }
}