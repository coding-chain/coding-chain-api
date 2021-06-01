using System;
using Application.Common.Exceptions;
using Application.Contracts.IService;
using Application.Contracts.Processes;
using Application.Write.ParticipationsSessions;
using CodingChainApi.Infrastructure.MessageBroker;
using CodingChainApi.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodingChainApi.Infrastructure.Services.Processes
{
    public record ProcessEndResult(Guid ParticipationId, string? Errors, string? Output);

    public class ParticipationExecutionService : RabbitMqBaseListener<ParticipationExecutionService>,
        IParticipationExecutionService
    {
        private readonly IRabbitMqPublisher _rabbitMqPublisher;

        private readonly IMediator _mediator;
        
        protected sealed override string QueueName { get; set; }
        protected sealed override string RouteKey { get; set; }

        public ParticipationExecutionService(IRabbitMQSettings settings, ILogger<ParticipationExecutionService> logger,
            IRabbitMqPublisher rabbitMqPublisher, IMediator mediator)
            : base(settings, logger)
        {
            _rabbitMqPublisher = rabbitMqPublisher;
            _mediator = mediator;
            RouteKey = settings.RoutingKey;
            QueueName = settings.ExecutionCodeRoute;
        }


        public void StartExecution(RunParticipationTestsDto command)
        {
            _rabbitMqPublisher.PushMessage(QueueName, command);
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
                if (result is null)
                {
                    Logger.LogError("Cannot deserialize process end message {Message}", json.ToString());
                }
                else
                {
                    _mediator.Publish(new UpdateParticipationProcessNotification(result.ParticipationId, result.Errors,
                        result.Output));
                }
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