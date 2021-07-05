using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Participations;
using Domain.StepEditions;
using Domain.Teams;
using Domain.Tournaments;
using Domain.Users;
using MediatR;
using ApplicationException = Application.Common.Exceptions.ApplicationException;
using StepEntity = Domain.Participations.StepEntity;

namespace Application.Write.Participations
{
    public record CreateParticipationCommand(Guid TournamentId, Guid StepId, Guid TeamId) : IRequest<string>;

    public class CreateParticipationHandler : IRequestHandler<CreateParticipationCommand, string>

    {
        private readonly IParticipationRepository _participationRepository;
        private readonly IReadParticipationRepository _readParticipationRepository;
        private readonly IReadStepRepository _readStepRepository;
        private readonly IReadTeamRepository _readTeamRepository;
        private readonly IReadTournamentRepository _readTournamentRepository;
        private readonly ITimeService _timeService;

        public CreateParticipationHandler(IParticipationRepository participationRepository,
            IReadTournamentRepository readTournamentRepository, IReadStepRepository readStepRepository,
            IReadTeamRepository readTeamRepository, ITimeService timeService,
            IReadParticipationRepository readParticipationRepository)
        {
            _participationRepository = participationRepository;
            _readTournamentRepository = readTournamentRepository;
            _readStepRepository = readStepRepository;
            _readTeamRepository = readTeamRepository;
            _timeService = timeService;
            _readParticipationRepository = readParticipationRepository;
        }


        public async Task<string> Handle(CreateParticipationCommand request, CancellationToken cancellationToken)
        {
            var tournamentNavigation =
                await _readTournamentRepository.GetOneTournamentNavigationById(request.TournamentId);
            if (tournamentNavigation is null)
                throw new NotFoundException(request.TournamentId.ToString(), "Tournament");
            var stepNavigation = await _readStepRepository.GetOneStepNavigationById(request.StepId);
            if (stepNavigation is null)
                throw new NotFoundException(request.StepId.ToString(), "Step");
            var teamNavigation = await _readTeamRepository
                .GetOneTeamNavigationByIdAsync(request.TeamId);
            if (teamNavigation is null)
                throw new NotFoundException(request.TeamId.ToString(), "Team");
            if (await _readParticipationRepository.ParticipationExistsByTournamentStepTeamIds(request.TournamentId,
                request.StepId, request.TeamId))
                throw new ApplicationException(
                    $"Participation for team : {request.TeamId} on tournament : {request.TournamentId} on step : {request.StepId} already exists");

            if (!await CanAddParticipation(request.TeamId, request.TournamentId, request.StepId))
                throw new ApplicationException("Participation cannot be created, finish previous first");

            var team = new TeamEntity(new TeamId(request.TeamId),
                teamNavigation.MembersIds.Select(memberId => new UserId(memberId)).ToList());
            var step = new StepEntity(new StepId(request.StepId),
                stepNavigation.TournamentsIds.Select(tournamentId => new TournamentId(tournamentId)).ToList());
            var tournament =
                new TournamentEntity(new TournamentId(request.TournamentId), tournamentNavigation.IsPublished);
            var participation = ParticipationAggregate.CreateNew(await _participationRepository.NextIdAsync(), team,
                tournament, step, _timeService.Now());

            await _participationRepository.SetAsync(participation);
            return participation.Id.ToString();
        }

        private async Task<bool> CanAddParticipation(Guid teamId, Guid tournamentId, Guid stepId)
        {
            var teamParticipations =
                await _readParticipationRepository.GetAllParticipationsByTeamAndTournamentId(teamId, tournamentId);
            var tournamentSteps =
                (await _readTournamentRepository.GetAllTournamentStepNavigationByTournamentId(tournamentId)).ToList();
            tournamentSteps.Sort((s1, s2) => s1.Order - s2.Order);
            var stepIdx = tournamentSteps.FindIndex(tS => tS.StepId == stepId);
            if (stepIdx == 0)
                return true;
            for (var i = stepIdx - 1; i >= 0; i--)
            {
                if (tournamentSteps[i].IsOptional)
                    continue;
                var previousParticipation =
                    teamParticipations.FirstOrDefault(s => s.StepId == tournamentSteps[i].StepId);
                if (previousParticipation?.EndDate is null)
                    return false;
            }

            return true;
        }
    }
}