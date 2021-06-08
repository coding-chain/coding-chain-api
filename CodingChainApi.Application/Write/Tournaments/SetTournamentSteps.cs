using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.StepEditions;
using Domain.Tournaments;
using MediatR;

namespace Application.Write.Tournaments
{
    public record TournamentStep(Guid StepId, bool IsOptional, int Order);

    public record SetTournamentStepsCommand(Guid TournamentId, IList<TournamentStep> Steps) : IRequest<string>;

    public class SetTournamentSteps : IRequestHandler<SetTournamentStepsCommand, string>
    {
        private readonly IReadStepRepository _readStepRepository;
        private readonly ITournamentRepository _tournamentRepository;

        public SetTournamentSteps(IReadStepRepository readStepRepository, ITournamentRepository tournamentRepository)
        {
            _readStepRepository = readStepRepository;
            _tournamentRepository = tournamentRepository;
        }

        public async Task<string> Handle(SetTournamentStepsCommand request, CancellationToken cancellationToken)
        {
            foreach (var step in request.Steps)
                if (!await _readStepRepository.StepExistsById(step.StepId))
                    throw new NotFoundException(step.StepId.ToString(), nameof(StepEntity));
            var tournament = await _tournamentRepository.FindByIdAsync(new TournamentId(request.TournamentId));
            if (tournament is null)
                throw new NotFoundException(request.TournamentId.ToString(), nameof(TournamentAggregate));

            var steps = request.Steps
                .Select(s => new StepEntity(new StepId(s.StepId), s.Order, s.IsOptional))
                .ToList();
            tournament.SetSteps(steps);
            await _tournamentRepository.SetAsync(tournament);
            return request.TournamentId.ToString();
        }
    }
}