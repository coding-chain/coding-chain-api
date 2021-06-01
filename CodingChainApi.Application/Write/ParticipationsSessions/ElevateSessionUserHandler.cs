using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.Participations;
using Domain.ParticipationStates;
using Domain.Users;
using MediatR;

namespace Application.Write.ParticipationsSessions
{
    [Authenticated]
    public record ElevateSessionUserCommand(Guid ParticipationId, Guid UserId) : IRequest<string>;

    public class ElevateSessionUserHandler : IRequestHandler<ElevateSessionUserCommand, string>
    {
        private readonly IParticipationsSessionsRepository _repository;
        private readonly ICurrentUserService _userService;

        public ElevateSessionUserHandler(IParticipationsSessionsRepository repository, ICurrentUserService userService,
            ITimeService timeService)
        {
            _repository = repository;
            _userService = userService;
        }

        public async Task<string> Handle(ElevateSessionUserCommand request, CancellationToken cancellationToken)
        {
            var participation = await _repository.FindByIdAsync(new ParticipationId(request.ParticipationId));
            if (participation is null)
                throw new NotFoundException(request.ParticipationId.ToString(), "ParticipationSession");
            participation.ElevateUser(new UserId(request.UserId), _userService.ConnectedUserId);
            await _repository.SetAsync(participation);
            return request.UserId.ToString();
        }
    }
}