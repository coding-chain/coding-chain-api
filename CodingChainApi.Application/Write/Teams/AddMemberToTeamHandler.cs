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
    public record AddMemberToTeamCommand(Guid TeamId, Guid MemberId ): IRequest<string>;

    public class AddMemberToTeamHandler: IRequestHandler<AddMemberToTeamCommand, string>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IReadUserRepository _readUserRepository;
        private readonly ICurrentUserService _currentUserService;

        public AddMemberToTeamHandler(ITeamRepository teamRepository, IReadUserRepository readUserRepository, ICurrentUserService currentUserService)
        {
            _teamRepository = teamRepository;
            _readUserRepository = readUserRepository;
            _currentUserService = currentUserService;
        }


        public async Task<string> Handle(AddMemberToTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.FindByIdAsync(new TeamId(request.TeamId));
            
            if (team is null)
            {
                throw new ApplicationException($"Team with id {request.TeamId} not found");
            }
            
            team.ValidateMemberAdditionByMember(_currentUserService.UserId);
            
            if(!await  _readUserRepository.UserExistsByIdAsync(request.MemberId))
                throw new ApplicationException($"User with id {request.MemberId} not found");
            
            
            var newMember = new MemberEntity(new UserId(request.MemberId),false); 
            team.AddMember(newMember);
            await _teamRepository.SetAsync(team);
            return newMember.Id.ToString();
        }
    }
}