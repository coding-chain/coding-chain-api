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
    public record AddFunctionParticipationSessionCommand(Guid ParticipationId, string Code,
        int? Order): IRequest<string>;
    public class AddFunctionToParticipationHandler: IRequestHandler<AddFunctionParticipationSessionCommand, string>
    {
        private readonly IParticipationsSessionsRepository _repository;
        private readonly ICurrentUserService _userService;
        private readonly ITimeService _timeService;

        public AddFunctionToParticipationHandler(IParticipationsSessionsRepository repository, ICurrentUserService userService, ITimeService timeService)
        {
            _repository = repository;
            _userService = userService;
            _timeService = timeService;
        }

        public async Task<string> Handle(AddFunctionParticipationSessionCommand request, CancellationToken cancellationToken)
        {
            var participation = await _repository.FindByIdAsync(new ParticipationId(request.ParticipationId));
            if(participation is null)
                throw new NotFoundException(request.ParticipationId.ToString(), "ParticipationSession");
            var function = new FunctionEntity(
                await _repository.GetNextFunctionId(),
                _userService.ConnectedUserId,
                request.Code,
                _timeService.Now(),
                request.Order
                );
            participation.AddFunction(function, _userService.ConnectedUserId);
            await _repository.SetAsync(participation);
            return function.Id.ToString();
        }
    }
    

}