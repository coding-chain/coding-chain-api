using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.IService;
using Application.Contracts.Processes;
using MediatR;

namespace Application.Write.Code.CodeExecution
{
    public record RunParticipationTestsCommand(Guid ParticipationId, string Language, string HeaderCode,
        IList<RunParticipationTestsCommand.Test> Tests,
        IList<RunParticipationTestsCommand.Function> Functions) : IRequest<IProcessEndHandler>
    {
        public record Test(string OutputValidator, string InputGenerator);

        public record Function(string Code, int Order);
    }

    public class CodeExecutionHandler : IRequestHandler<RunParticipationTestsCommand, IProcessEndHandler>
    {
        private readonly IParticipationExecutionService _participationExecutionService;

        public CodeExecutionHandler(IParticipationExecutionService participationExecutionService)
        {
            _participationExecutionService = participationExecutionService;
        }

        public async Task<IProcessEndHandler> Handle(RunParticipationTestsCommand request,
            CancellationToken cancellationToken)
        {
            _participationExecutionService.StartExecution(request);
            return _participationExecutionService.FollowExecution(request.ParticipationId);
        }
    }
}