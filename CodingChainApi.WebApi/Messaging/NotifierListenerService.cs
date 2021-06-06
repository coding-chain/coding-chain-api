using System;
using CodingChainApi.Infrastructure.MessageBroker;
using CodingChainApi.Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeosCodingApi.Messaging
{
    public abstract class NotifierListenerService<TCommand> : RabbitMqBaseListener where TCommand : INotification
    {
        private readonly IPublisher _mediator;

        protected NotifierListenerService(IRabbitMqSettings settings, ILogger<NotifierListenerService<TCommand>> logger,
            IPublisher mediator) : base(settings, logger)
        {
            _mediator = mediator;
        }

        public override bool Process(string message)
        {
            try
            {
                var json = JObject.Parse(message);
                var runParticipation = JsonConvert.DeserializeObject<TCommand>(json.ToString());
                if (runParticipation is null)
                {
                    Logger.LogError("Cannot parse rabbitmq message");
                    return false;
                }

                _mediator.Publish(runParticipation);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("Process fail,error:{ExceptionMessage},stackTrace:{ex.StackTrace},message:{Message}",
                    ex.Message, message);
                Logger.LogError(-1, ex, "Process fail");
                return false;
            }
        }
    }
}