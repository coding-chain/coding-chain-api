using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.Teams;
using MediatR;
using ApplicationException = Application.Common.Exceptions.ApplicationException;

namespace Application.Write.Teams
{
    [Authenticated]
    public record RenameTeamCommand(Guid TeamId, string Name) : IRequest<string>;

    public class RenameTeamHandler : IRequestHandler<RenameTeamCommand, string>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITeamRepository _teamRepository;

        public RenameTeamHandler(ITeamRepository teamRepository, ICurrentUserService currentUserService)
        {
            _teamRepository = teamRepository;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(RenameTeamCommand request, CancellationToken cancellationToken)
        {
            var (teamId, name) = request;
            var team = await _teamRepository.FindByIdAsync(new TeamId(teamId));
            if (team is null)
                throw new ApplicationException($"Team with id {teamId} doesn't exists");
            team.ValidateTeamRenamingByMember(_currentUserService.UserId);
            team.Rename(name);
            await _teamRepository.SetAsync(team);
            return team.Id.ToString();
        }
    }
}