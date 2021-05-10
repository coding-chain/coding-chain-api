using System;
using Application.Common.MessageBroker.RabbitMQ;
using Application.Contracts.IService;
using Application.Contracts.Processes;
using Application.Write.Code.CodeExecution;
using CodingChainApi.Infrastructure.MessageBroker.RabbitMQ;

namespace CodingChainApi.Infrastructure.Services.Processes
{
    public class ParticipationExecutionService : RabbitMqBaseListener, IParticipationExecutionService
    {
        private readonly IRabbitMQPublisher _rabbitMqPublisher;

        public ParticipationExecutionService(IRabbitMQPublisher rabbitMqPublisher)
        {
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public void StartExecution(RunParticipationTestsCommand command)
        {
            _rabbitMqPublisher.PushMessage("code.execution.pending", command);
        }

        public IProcessEndHandler FollowExecution(Guid participationId)
        {
            return new ProcessEndHandler();
        }

        public override bool Process(string message)
        {
            throw new NotImplementedException();
        }
    }
}