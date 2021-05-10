using System;
using System.Text;
using CodingChainApi.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CodingChainApi.Infrastructure.MessageBroker.RabbitMQ
{
    public class RabbitMQPublisher : IRabbitMqPublisher
    {
        private readonly IModel? _channel;

        private readonly ILogger _logger;

        private readonly string? _routeKey;
        private readonly string? _queueWorker;

        public RabbitMQPublisher(IRabbitMQSettings settings, ILogger<RabbitMQPublisher> logger)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = settings.RabbitHost
                };
                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();
                _routeKey = settings.RoutingKey;
                _queueWorker = settings.RabbitMqWorker;
            }
            catch (Exception ex)
            {
                logger.LogError(-1, ex, "RabbitMQClient init fail");
            }

            _logger = logger;
        }

        public virtual void PushMessage(string queueName, object message)
        {
            _logger.LogInformation($"PushMessage,queueName:{queueName}");
            _channel?.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: true,
                arguments: null);

            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);
            _channel.BasicPublish(exchange: _queueWorker,
                routingKey: _routeKey,
                basicProperties: null,
                body: body);
        }
    }
}