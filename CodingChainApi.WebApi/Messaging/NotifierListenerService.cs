using System;
using CodingChainApi.Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodingChainApi.Messaging
{
    public abstract class NotifierListenerService<TCommand> : RabbitMqBaseListener where TCommand : INotification
    {
        private readonly IServiceProvider _serviceProvider;
        protected NotifierListenerService(IRabbitMqSettings settings, ILogger<NotifierListenerService<TCommand>> logger, IServiceProvider serviceProvider) : base(settings, logger)
        {
            _serviceProvider = serviceProvider;
        }

        public override bool Process(string message)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                Logger.LogInformation(
                    "RabbitListener message received on , exchange: {Exchange}, routeKey:{RoutingKey}",
                    Exchange, RoutingKey);
                var json = JObject.Parse(message);
                var runParticipation = JsonConvert.DeserializeObject<TCommand>(json.ToString());
                if (runParticipation is null)
                {
                    Logger.LogError("Cannot parse rabbitmq message");
                    return false;
                }

                mediator.Publish(runParticipation);
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