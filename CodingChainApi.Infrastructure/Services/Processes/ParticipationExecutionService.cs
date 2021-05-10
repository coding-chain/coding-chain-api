using System;
using System.Collections;
using System.Collections.Generic;
using Application.Common.MessageBroker.RabbitMQ;
using Application.Contracts.IService;
using Application.Contracts.Processes;
using Application.Write.Code.CodeExecution;
using CodingChainApi.Infrastructure.MessageBroker.RabbitMQ;
using CodingChainApi.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ILogger = Castle.Core.Logging.ILogger;

namespace CodingChainApi.Infrastructure.Services.Processes
{
    public class ParticipationExecutionService : RabbitMqBaseListener<ParticipationExecutionService>,
        IParticipationExecutionService
    {
        private readonly IRabbitMQPublisher _rabbitMqPublisher;

        private ProcessEndHandler<Guid>? _processEndHandler = null;

        public ParticipationExecutionService(IRabbitMQSettings settings, ILogger<ParticipationExecutionService> logger, IRabbitMQPublisher rabbitMqPublisher)
            : base(settings, logger)
        {
            _rabbitMqPublisher = rabbitMqPublisher;
            RouteKey = settings.CodeRouteKey;
            QueueName = settings.ExecutionCodeRoute;
        }

        public void StartExecution(RunParticipationTestsCommand command)
        {
            _rabbitMqPublisher.PushMessage("code.execution.pending", command);
            _processEndHandler = new ProcessEndHandler<Guid>(command.ParticipationId);
        }

        public IProcessEndHandler? FollowExecution(Guid participationId)
        {
            return _processEndHandler;
        }

        public override bool Process(string message)
        {
            var taskMessage = message;
            if (taskMessage == null)
            {
                // When false is returned, the message is rejected directly, indicating that it cannot be processed
                return false;
            }

            try
            {
                _logger.LogDebug($"message: ${taskMessage}");
                var json = JObject.Parse(taskMessage);
                var result = JsonConvert.DeserializeObject<ProcessEndResult>(json.ToString());

                _processEndHandler?.AddError(result?.Errors);
                _processEndHandler?.AddOutput(result?.Output);
                _processEndHandler?.End();


                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Process fail,error:{ex.Message},stackTrace:{ex.StackTrace},message:{message}");
                _logger.LogError(-1, ex, "Process fail");
                return false;
            }
        }
    }
}