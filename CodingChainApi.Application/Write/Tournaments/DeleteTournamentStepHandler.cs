using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Write.Contracts;
using Domain.StepEditions;
using Domain.Tournaments;
using MediatR;

namespace Application.Write.Tournaments
{
    public record DeleteTournamentStepCommand(Guid TournamentId, Guid StepId) : IRequest<string>;

    public class DeleteTournamentStepHandler : IRequestHandler<DeleteTournamentStepCommand, string>
    {
        private readonly ITournamentRepository _tournamentRepository;

        public DeleteTournamentStepHandler(ITournamentRepository tournamentRepository)
        {
            _tournamentRepository = tournamentRepository;
        }


        public async Task<string> Handle(DeleteTournamentStepCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _tournamentRepository.FindByIdAsync(new TournamentId(request.TournamentId));
            if (tournament is null)
                throw new NotFoundException(request.TournamentId.ToString(), nameof(TournamentAggregate));
            tournament.RemoveStep(new StepId(request.StepId));
            await _tournamentRepository.SetAsync(tournament);
            return tournament.Id.ToString();
        }
    }
}