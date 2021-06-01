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
    public record UpdateParticipationSessionFunctionCommand(Guid ParticipationId, Guid FunctionId, string Code,
        int? Order): IRequest<string>;
    public class UpdateParticipationSessionFunctionHandler: IRequestHandler<UpdateParticipationSessionFunctionCommand, string>
    {
        private readonly IParticipationsSessionsRepository _repository;
        private readonly ICurrentUserService _userService;
        private readonly ITimeService _timeService;

        public UpdateParticipationSessionFunctionHandler(IParticipationsSessionsRepository repository, ICurrentUserService userService, ITimeService timeService)
        {
            _repository = repository;
            _userService = userService;
            _timeService = timeService;
        }

        public async Task<string> Handle(UpdateParticipationSessionFunctionCommand request, CancellationToken cancellationToken)
        {
            var participation = await _repository.FindByIdAsync(new ParticipationId(request.ParticipationId));
            if(participation is null)
                throw new NotFoundException(request.ParticipationId.ToString(), "ParticipationSession");
            var function = new FunctionEntity(
                new FunctionId(request.FunctionId),
                _userService.ConnectedUserId,
                request.Code,
                _timeService.Now(),
                request.Order
            );
            participation.UpdateFunction(function, _userService.ConnectedUserId);
            await _repository.SetAsync(participation);
            return function.Id.ToString();
        }
    }
}