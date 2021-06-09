using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Write.Contracts;
using Domain.Tournaments;
using MediatR;

namespace Application.Write.Tournaments
{
    public record UpdateTournamentCommand(Guid TournamentId, string Name, string Description, bool IsPublished,
        DateTime? StartDate,
        DateTime? EndDate) : IRequest<string>;

    public class UpdateTournamentHandler : IRequestHandler<UpdateTournamentCommand, string>
    {
        private readonly ITournamentRepository _tournamentRepository;

        public UpdateTournamentHandler(ITournamentRepository tournamentRepository)
        {
            _tournamentRepository = tournamentRepository;
        }

        public async Task<string> Handle(UpdateTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _tournamentRepository.FindByIdAsync(new TournamentId(request.TournamentId));
            if (tournament is null)
                throw new NotFoundException(request.TournamentId.ToString(), nameof(TournamentAggregate));
            tournament.Update(request.Name, request.Description, request.IsPublished, request.StartDate,
                request.EndDate);
            await _tournamentRepository.SetAsync(tournament);
            return tournament.Id.ToString();
        }
    }
}