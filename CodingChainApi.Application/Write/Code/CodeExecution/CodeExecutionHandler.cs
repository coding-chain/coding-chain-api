using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Write.Code.CodeExecution
{
    public record RunParticipationTestsCommand(Guid ParticipationId, string Language, string HeaderCode,
        IList<RunParticipationTestsCommand.Test> Tests,
        IList<RunParticipationTestsCommand.Function> Functions) : IRequest<string>
    {
        public record Test(string OutputValidator, string InputGenerator);

        public record Function(string Code, int Order);
    }

    public class CodeExecutionHandler : IRequestHandler<ICodeExecutionRecords.RunParticipationTestsCommand, string>
    {
        private RabbitMQPublisher _rabbitMqPublisher;

        public CodeExecutionHandler(RabbitMQPublisher rabbitMqPublisher)
        {
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public async Task<string> Handle(ICodeExecutionRecords.RunParticipationTestsCommand request,
            CancellationToken cancellationToken)
        {
            var codeExecutionPublisher = _rabbitMqPublisher;
            codeExecutionPublisher?.PushMessage("code.execution.pending", request);

            return request.ParticipationId.ToString();
        }
    }
}