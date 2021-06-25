using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Events;
using Application.Common.Exceptions;
using Application.Contracts.Dtos;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Domain.Participations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Write.Plagiarism
{
    public class
        ParticipationSuspectFunctionRemovedHandler : INotificationHandler<
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

            var functionRepository = scope.ServiceProvider.GetRequiredService<IReadFunctionRepository>();
            var function = await functionRepository.GetById(notification.DomainEvent.FunctionId.Value, true);
            if (function is null)
                throw new NotFoundException(notification.DomainEvent.FunctionId.ToString(), "Function");

            var userId = await functionRepository.GetLastEditorIdById(function.Id, true);
            if (userId is null)
                throw new NotFoundException(function.Id.ToString(), "FunctionUser");

            var readUserRepository = scope.ServiceProvider.GetRequiredService<IReadUserRepository>();
            var user = await readUserRepository.FindPublicUserById(userId.Value);
            if (user is null)
                throw new NotFoundException(userId.Value.ToString(), "User");

            var readParticipationRepository = scope.ServiceProvider.GetRequiredService<IReadParticipationRepository>();
            var participation =
                await readParticipationRepository.GetOneParticipationNavigationById(function.ParticipationId);
            if (participation is null)
                throw new NotFoundException(function.ParticipationId.ToString(), "Participation");

            var readTeamRepository = scope.ServiceProvider.GetRequiredService<IReadTeamRepository>();
            var team = await readTeamRepository.GetOneTeamNavigationByIdAsync(participation.TeamId);
            if (team is null)
                throw new NotFoundException(participation.TeamId.ToString(), "Team");

            var readTournamentRepository = scope.ServiceProvider.GetRequiredService<IReadTournamentRepository>();
            var tournament = await readTournamentRepository.GetOneTournamentNavigationById(participation.TournamentId);
            if (tournament is null)
                throw new NotFoundException(participation.TournamentId.ToString(), "Tournament");

            var readStepRepository = scope.ServiceProvider.GetRequiredService<IReadStepRepository>();
            var step = await readStepRepository.GetOneStepNavigationById(participation.StepId);
            if (step is null)
                throw new NotFoundException(participation.StepId.ToString(), "Step");

            var mailService = scope.ServiceProvider.GetRequiredService<IMailService<SuspectFunctionContent>>();
            await mailService.SendMessage(
                new Message<SuspectFunctionContent>(
                    new Contact(
                        user.Email,
                        user.Username
                    ),
                    "Nous avons détécté un plagiat de votre part !",
                    new SuspectFunctionContent(function, tournament, team, step)
                )
            );
        }
    }
}