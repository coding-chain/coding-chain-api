using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Teams;
using Domain.Users;
using MediatR;

namespace Application.Write.Teams
{
    [Authenticated]
    public record DeleteTeamCommand(Guid TeamId) : IRequest<string>;

    public class DeleteTeamHandler: IRequestHandler<DeleteTeamCommand, string>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteTeamHandler(ITeamRepository teamRepository, ICurrentUserService currentUserService)
        {
            _teamRepository = teamRepository;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.FindByIdAsync(new TeamId(request.TeamId));
            if (team is null)
                throw new ApplicationException($"Team with id {request.TeamId} doesn't exists");
            team.ValidateTeamDeletionByMember(_currentUserService.UserId);
            await _teamRepository.RemoveAsync(team.Id);
            return team.Id.ToString();
        }
    }
}