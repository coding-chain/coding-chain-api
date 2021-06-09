using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.Participations;
using MediatR;

namespace Application.Write.ParticipationsSessions
{
    [Authenticated]
    public record RemoveFunctionParticipationSessionCommand(Guid ParticipationId, Guid FunctionId) : IRequest<string>;

    public class
        RemoveFunctionFromParticipationHandler : IRequestHandler<RemoveFunctionParticipationSessionCommand, string>
    {
        private readonly IParticipationsSessionsRepository _repository;
        private readonly ICurrentUserService _userService;

        public RemoveFunctionFromParticipationHandler(IParticipationsSessionsRepository repository,
            ICurrentUserService userService, ITimeService timeService)
        {
            _repository = repository;
            _userService = userService;
        }

        public async Task<string> Handle(RemoveFunctionParticipationSessionCommand request,
            CancellationToken cancellationToken)
        {
            var participation = await _repository.FindByIdAsync(new ParticipationId(request.ParticipationId));
            if (participation is null)
                throw new NotFoundException(request.ParticipationId.ToString(), "ParticipationSession");
            participation.RemoveFunction(new FunctionId(request.FunctionId), _userService.UserId);
            return (await _repository.SetAsync(participation)).ToString();
        }
    }
}