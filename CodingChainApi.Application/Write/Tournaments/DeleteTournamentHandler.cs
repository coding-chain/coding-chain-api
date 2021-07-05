using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.Tournaments;
using MediatR;

namespace Application.Write.Tournaments
{
    public record DeleteTournamentCommand(Guid TournamentId) : IRequest<string>;

    public class DeleteTournamentHandler : IRequestHandler<DeleteTournamentCommand, string>
    {
        private readonly ITimeService _timeService;
        private readonly ITournamentRepository _tournamentRepository;

        public DeleteTournamentHandler(ITournamentRepository tournamentRepository, ITimeService timeService)
        {
            _tournamentRepository = tournamentRepository;
            _timeService = timeService;
        }

        public async Task<string> Handle(DeleteTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _tournamentRepository.FindByIdAsync(new TournamentId(request.TournamentId));
            if (tournament is null)
                throw new NotFoundException(request.TournamentId.ToString(), nameof(TournamentAggregate));
            tournament.ValidateDeletion(_timeService.Now());
            await _tournamentRepository.RemoveAsync(tournament.Id);
            return tournament.Id.ToString();
        }
    }
}