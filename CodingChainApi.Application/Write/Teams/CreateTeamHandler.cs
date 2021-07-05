using System.Threading;
using System.Threading.Tasks;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Teams;
using MediatR;

namespace Application.Write.Teams
{
    [Authenticated]
    public record CreateTeamCommand(string Name) : IRequest<string>;

    public class CreateTeamHandler : IRequestHandler<CreateTeamCommand, string>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IReadUserRepository _readUserRepository;
        private readonly ITeamRepository _teamRepository;

        public CreateTeamHandler(ITeamRepository teamRepository, IReadUserRepository readUserRepository,
            ICurrentUserService currentUserService)
        {
            _teamRepository = teamRepository;
            _readUserRepository = readUserRepository;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var adminMember = new MemberEntity(_currentUserService.UserId, true);
            var team = TeamAggregate.CreateNew(await _teamRepository.NextIdAsync(), request.Name, adminMember);
            await _teamRepository.SetAsync(team);
            return team.Id.ToString();
        }
    }
}