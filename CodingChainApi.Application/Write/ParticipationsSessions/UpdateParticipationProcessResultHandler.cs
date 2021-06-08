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
    public record ProcessEndNotification
        (Guid ParticipationId, string? Output, string? Error, IList<Guid> TestsPassedIds) : INotification;

    public class UpdateParticipationProcessResultHandler : INotificationHandler<ProcessEndNotification>
    {
        private readonly IParticipationsSessionsRepository _participationsSessionsRepository;
        private readonly ITimeService _timeService;

        public UpdateParticipationProcessResultHandler(IParticipationsSessionsRepository participationsSessionsRepository, ITimeService timeService)
        {
            _participationsSessionsRepository = participationsSessionsRepository;
            _timeService = timeService;
        }

        public async Task Handle(ProcessEndNotification notification,
            CancellationToken cancellationToken)
        {

            var participation =
                await _participationsSessionsRepository.FindByIdAsync(
                    new ParticipationId(notification.ParticipationId));
            if (participation is null)
                throw new NotFoundException(notification.ParticipationId.ToString(), "Participation");

            var testsPassedIds = notification.TestsPassedIds.Select(id => new TestId(id)).ToList();
            participation.SetProcessResult(notification.Error, notification.Output, testsPassedIds, _timeService.Now());
            await _participationsSessionsRepository.SetAsync(participation);
        }
    }
}