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
using StepEntity = Domain.Participations.StepEntity;

namespace Application.Write.Participations
{
    public record CreateParticipationCommand(Guid TournamentId, Guid StepId, Guid TeamId) : IRequest<string>;

    public class CreateParticipationHandler : IRequestHandler<CreateParticipationCommand, string>

    {
        private readonly IParticipationRepository _participationRepository;
        private readonly IReadTournamentRepository _readTournamentRepository;
        private readonly IReadTeamRepository _readTeamRepository;
        private readonly IReadStepRepository _readStepRepository;
        private readonly ITimeService _timeService;

        public CreateParticipationHandler(IParticipationRepository participationRepository,
            IReadTournamentRepository readTournamentRepository, IReadStepRepository readStepRepository,
            IReadTeamRepository readTeamRepository, ITimeService timeService)
        {
            _participationRepository = participationRepository;
            _readTournamentRepository = readTournamentRepository;
            _readStepRepository = readStepRepository;
            _readTeamRepository = readTeamRepository;
            _timeService = timeService;
        }


        public async Task<string> Handle(CreateParticipationCommand request, CancellationToken cancellationToken)
        {
            var tournamentNavigation = await _readTournamentRepository.GetOneTournamentNavigationById(request.TournamentId);
            if(tournamentNavigation is null)
                throw new NotFoundException(request.TournamentId.ToString(), "Tournament");
            var stepNavigation = await _readStepRepository.GetOneStepNavigationById(request.StepId);
            if(stepNavigation is null)
                throw new NotFoundException(request.StepId.ToString(), "Step");
            var teamNavigation = await _readTeamRepository
                .GetOneTeamNavigationByIdAsync(request.TeamId);
            if(teamNavigation is null)
                throw new NotFoundException(request.TeamId.ToString(), "Team");
            
            var team = new TeamEntity(new TeamId(request.TeamId), teamNavigation.MembersIds.Select(memberId => new UserId(memberId)).ToList());
            var step = new StepEntity(new StepId(request.StepId),
                stepNavigation.TournamentsIds.Select(tournamentId => new TournamentId(tournamentId)).ToList());
            var tournament = new TournamentEntity(new TournamentId(request.TournamentId), tournamentNavigation.IsPublished);
            var participation = ParticipationAggregate.CreateNew(await _participationRepository.NextIdAsync(), team,
                tournament, step, _timeService.Now());
            
            await _participationRepository.SetAsync(participation);
            return participation.Id.ToString();
        }
    }
}