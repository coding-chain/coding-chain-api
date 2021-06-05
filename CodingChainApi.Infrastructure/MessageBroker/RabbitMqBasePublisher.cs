using System;
using System.Text;
using CodingChainApi.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CodingChainApi.Infrastructure.MessageBroker
{
    public  abstract class RabbitMqBasePublisher
    {
        private readonly IModel? _channel;

        private readonly ILogger _logger;

        protected string? RoutingKey;
        protected string? Exchange;

        public RabbitMqBasePublisher(IRabbitMqSettings settings, ILogger<RabbitMqBasePublisher> logger)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = settings.RabbitHost
                };
                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                logger.LogError(-1, ex, "RabbitMQClient init fail");
            }

            _logger = logger;
        }

        protected void PushMessage(object message)
        {
            _logger.LogInformation("PushMessage in {Exchange}, routing key:{RoutingKey}", Exchange, RoutingKey);
            _channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType.Topic);

            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);
            _channel.BasicPublish(exchange: Exchange,
                routingKey: RoutingKey,
                basicProperties: null,
                body: body);
        }
    }
}