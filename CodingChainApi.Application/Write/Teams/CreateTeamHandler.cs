using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Teams;
using Domain.Users;
using MediatR;
using ApplicationException = Application.Common.Exceptions.ApplicationException;

namespace Application.Write.Teams
{
    [Authenticated]
    public record CreateTeamCommand(string Name) : IRequest<string>;

    public class CreateTeamHandler : IRequestHandler<CreateTeamCommand, string>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IReadUserRepository _readUserRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateTeamHandler(ITeamRepository teamRepository, IReadUserRepository readUserRepository,
            ICurrentUserService currentUserService)
        {
            _teamRepository = teamRepository;
            _readUserRepository = readUserRepository;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var adminMember = new MemberEntity(_currentUserService.ConnectedUserId, true);
            var team = TeamAggregate.CreateNew(await _teamRepository.NextIdAsync(), request.Name, adminMember);
            await _teamRepository.SetAsync(team);
            return team.Id.ToString();
        }
    }
}