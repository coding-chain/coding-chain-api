using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.Teams;
using Domain.Users;
using MediatR;

namespace Application.Write.Teams
{
    [Authenticated]
    public record ElevateTeamMemberCommand(Guid TeamId, Guid TargetMemberId) : IRequest<string>;

    public class ElevateTeamMemberHandler : IRequestHandler<ElevateTeamMemberCommand, string>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ICurrentUserService _currentUserService;

        public ElevateTeamMemberHandler(ITeamRepository teamRepository, ICurrentUserService currentUserService)
        {
            _teamRepository = teamRepository;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(ElevateTeamMemberCommand request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.FindByIdAsync(new TeamId(request.TeamId));
            if (team is null)
                throw new ApplicationException($"Team with id {request.TeamId} doesn't exists");
            var memberId = new UserId(request.TargetMemberId);
            team.ValidateMemberElevationByMember(_currentUserService.ConnectedUserId);
            team.ElevateMember(memberId);
            await _teamRepository.SetAsync(team);
            return memberId.ToString();
        }
    }
}