using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.MessageBroker.RabbitMQ;
using Application.Common.MessageBroker.RabbitMQ.Code.CodeExecution;
using CodingChainApi.WebApi.Client.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NeosCodingApi.Records;

namespace Application.Publisher.Code.CodeExecution
{
    public class CodeExecutionHandler : IRequestHandler<CodeExecutionRecords.RunParticipationTestsCommand, string>
    {
        private IServiceProvider _service;

        public CodeExecutionHandler(IServiceProvider service)
        {
            _service = service;
        }

        public async Task<string> Handle(CodeExecutionRecords.RunParticipationTestsCommand request,
            CancellationToken cancellationToken)
        {
            var codeExecutionPublisher = _service.GetService<RabbitMQPublisher>();
            codeExecutionPublisher?.PushMessage("code.execution.pending", request);
            return request.ParticipationId.ToString();
        }
    }
}