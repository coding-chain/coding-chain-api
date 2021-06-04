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
    public record DeleteMemberFromTeamCommand(Guid TeamId, Guid MemberId) : IRequest<string>;

    public class DeleteMemberFromTeamHandler: IRequestHandler<DeleteMemberFromTeamCommand, string>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteMemberFromTeamHandler(ITeamRepository teamRepository, ICurrentUserService currentUserService)
        {
            _teamRepository = teamRepository;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(DeleteMemberFromTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.FindByIdAsync(new TeamId(request.TeamId));
            if (team is null)
                throw new ApplicationException($"Team with id {request.TeamId} doesn't exists");
            var memberId = new UserId(request.MemberId);
            team.ValidateMemberDeletionByMember(_currentUserService.UserId, memberId);
            team.RemoveMember(memberId);
            await _teamRepository.SetAsync(team);
            return memberId.ToString();
        }
    }
}