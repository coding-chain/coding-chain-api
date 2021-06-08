using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Participations;
using Domain.ParticipationSessions;
using Domain.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Write.ParticipationsSessions
{
    public class UpdateParticipationHandler : INotificationHandler<DomainEventNotification<ProcessResultUpdated>>
    {
        private readonly IParticipationRepository _participationRepository;
        private readonly IReadParticipationSessionRepository _readParticipationSessionRepository;
        private readonly IReadFunctionSessionRepository _readFunctionSessionRepository;

        public UpdateParticipationHandler(IParticipationRepository participationRepository, IReadParticipationSessionRepository readParticipationSessionRepository, IReadFunctionSessionRepository readFunctionSessionRepository)
        {
            _participationRepository = participationRepository;
            _readParticipationSessionRepository = readParticipationSessionRepository;
            _readFunctionSessionRepository = readFunctionSessionRepository;
        }

        public async Task Handle(DomainEventNotification<ProcessResultUpdated> notification,
            CancellationToken cancellationToken)
        {
            var participationId = notification.DomainEvent.ParticipationId.Value;
            var functions =
                await _readFunctionSessionRepository.GetAllAsync(participationId);
            var participationNav =
                await _readParticipationSessionRepository.GetOneById(participationId);
            var participation = await _participationRepository.FindByIdAsync(notification.DomainEvent.ParticipationId);
            var functionsEntities = functions.Select(f => new FunctionEntity(
                new FunctionId(f.Id),
                new UserId(f.UserId),
                f.Code,
                f.lastModificationDate,
                f.Order
            )).ToList();
            if (participationNav is null)
                throw new NotFoundException(participationId.ToString(), "ParticipationSession");

            if (participation is null) throw new NotFoundException(participationId.ToString(), "Participation");
            participation.Update(participationNav.EndDate, participationNav.CalculatedScore, functionsEntities);
            await _participationRepository.SetAsync(participation);
        }
    }
}