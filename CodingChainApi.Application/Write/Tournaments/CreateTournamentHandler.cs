using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Write.Contracts;
using Domain.Tournaments;
using MediatR;

namespace Application.Write.Tournaments
{
    public record CreateTournamentCommand(string Name, string Description): IRequest<string>;
    
    public class CreateTournamentHandler:IRequestHandler<CreateTournamentCommand, string>
    {
        private readonly ITournamentRepository _tournamentRepository;

        public CreateTournamentHandler(ITournamentRepository tournamentRepository)
        {
            _tournamentRepository = tournamentRepository;
        }

        public async Task<string> Handle(CreateTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = new TournamentAggregate(
                await _tournamentRepository.NextIdAsync(),
                request.Name, request.Description, false, null, null, new List<StepEntity>());
            await _tournamentRepository.SetAsync(tournament);
            return tournament.Id.ToString();
        }
    }
}