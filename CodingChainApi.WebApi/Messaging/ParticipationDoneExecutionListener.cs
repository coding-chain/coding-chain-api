using System;
using Application.Write.ParticipationsSessions;
using CodingChainApi.Infrastructure.MessageBroker;
using CodingChainApi.Infrastructure.Services.Processes;
using CodingChainApi.Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeosCodingApi.Messaging
{
    public class ParticipationDoneExecutionListener : RabbitMqBaseListener
    {
        private readonly IMediator _mediator;

        public ParticipationDoneExecutionListener(IRabbitMqSettings settings,
            ILogger<ParticipationDoneExecutionListener> logger, IMediator mediator) : base(settings, logger)
        {
            Exchange = settings.ParticipationExchange;
            RoutingKey = settings.DoneExecutionRoutingKey;
            _mediator = mediator;
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
                var json = JObject.Parse(message);
                var result = JsonConvert.DeserializeObject<ProcessEndResult>(json.ToString());
                if (result is null)
                {
                    Logger.LogError("Cannot deserialize process end message {Message}", json.ToString());
                }
                else
                {
                    _mediator.Publish(new UpdateParticipationProcessNotification(result.ParticipationId, result.Errors,
                        result.Output, result.TestsPassedIds));
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