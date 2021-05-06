using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Tournaments.Handlers
{
    public record GetTournamentNavigationByIdQuery(Guid Id) : IRequest<TournamentNavigation>;
    public class GetTournamentNavigationByIdHandler: IRequestHandler<GetTournamentNavigationByIdQuery, TournamentNavigation>
    {
        private readonly IReadTournamentRepository _readTournamentRepository;

        public GetTournamentNavigationByIdHandler(IReadTournamentRepository readTournamentRepository)
        {
            _readTournamentRepository = readTournamentRepository;
        }

        public async Task<TournamentNavigation> Handle(GetTournamentNavigationByIdQuery request, CancellationToken cancellationToken)
        {
            var tournament = await _readTournamentRepository.GetOneTournamentNavigationByID(request.Id);
            if (tournament is null)
                throw new ApplicationException($"Tournament {request.Id} not found");
            return tournament;
        }
    }
}