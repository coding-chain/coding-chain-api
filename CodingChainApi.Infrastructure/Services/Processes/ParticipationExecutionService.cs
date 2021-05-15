using System;
using Application.Common.Exceptions;
using Application.Contracts.IService;
using Application.Contracts.Processes;
using Application.Write.Code.CodeExecution;
using CodingChainApi.Infrastructure.MessageBroker;
using CodingChainApi.Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodingChainApi.Infrastructure.Services.Processes
{
    public class ParticipationExecutionService : RabbitMqBaseListener<ParticipationExecutionService>,
        IParticipationExecutionService
    {
        private readonly IRabbitMqPublisher _rabbitMqPublisher;

        private ProcessEndHandler<Guid>? _processEndHandler;
        protected sealed override string QueueName { get; set; }

        protected sealed override string RouteKey { get; set; }

        public ParticipationExecutionService(IRabbitMQSettings settings, ILogger<ParticipationExecutionService> logger,
            IRabbitMqPublisher rabbitMqPublisher)
            : base(settings, logger)
        {
            _rabbitMqPublisher = rabbitMqPublisher;
            RouteKey = settings.RoutingKey;
            QueueName = settings.ExecutionCodeRoute;
        }


        public void StartExecution(RunParticipationTestsCommand command)
        {
            _rabbitMqPublisher.PushMessage(QueueName, command);
            _processEndHandler = new ProcessEndHandler<Guid>(command.ParticipationId);
        }

        public IProcessEndHandler? FollowExecution(Guid participationId)
        {
            return _processEndHandler;
        }

        public override bool Process(string? message)
        {
            if (message == null)
            {
                // When false is returned, the message is rejected directly, indicating that it cannot be processed
                return false;
            }

            try
            {
                Logger.LogDebug("Code executed: ${Message}", message);
                var json = JObject.Parse(message);
                var result = JsonConvert.DeserializeObject<ProcessEndResult>(json.ToString());

                if (_processEndHandler == null)
                {
                    if (result is not null) _processEndHandler = new ProcessEndHandler<Guid>(result.ParticipationId);
                    else throw new InputFormatterException("Message in queue doesn't have any participation ID ! ");
                }

                _processEndHandler?.AddError(result?.Errors);
                _processEndHandler?.AddOutput(result?.Output);
                _processEndHandler?.End();


                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    "Process fail,error:{ExceptionMessage},stackTrace:{ExceptionStackTrace},message:{QueueMessage}",
                    ex.Message, ex.StackTrace, message);
                Logger.LogError(-1, ex, "Process fail");
                return false;
            }
        }
    }
}