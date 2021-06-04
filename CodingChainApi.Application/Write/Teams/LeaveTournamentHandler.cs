using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.Teams;
using Domain.Tournaments;
using MediatR;

namespace Application.Write.Teams
{
    [Authenticated]
    public record LeaveTournamentCommand(Guid TournamentId, Guid TeamId) : IRequest<string>;

    public class LeaveTournamentHandler : IRequestHandler<LeaveTournamentCommand, string>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ICurrentUserService _currentUserService;

        public LeaveTournamentHandler(ITeamRepository teamRepository, ICurrentUserService currentUserService)
        {
            _teamRepository = teamRepository;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(LeaveTournamentCommand request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.FindByIdAsync(new TeamId(request.TeamId));
            if (team is null)
                throw new ApplicationException($"Team with id {request.TeamId} doesn't exists");
            var tournamentId = new TournamentId(request.TournamentId);
            team.LeaveTournament(tournamentId, this._currentUserService.UserId);
            await _teamRepository.SetAsync(team);
            return team.Id.ToString();
        }
    }
}