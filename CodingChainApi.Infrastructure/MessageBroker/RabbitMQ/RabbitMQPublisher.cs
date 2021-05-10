using System;
using System.Text;
using Application.Common.MessageBroker.RabbitMQ;
using CodingChainApi.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CodingChainApi.Infrastructure.MessageBroker.RabbitMQ
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IModel _channel;

        private readonly ILogger _logger;

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
            }
            catch (Exception ex)
            {
                logger.LogError(-1, ex, "RabbitMQClient init fail");
            }

            _logger = logger;
        }

        public virtual void PushMessage(string routingKey, object message)
        {
            _logger.LogInformation($"PushMessage,routingKey:{routingKey}");
            _channel.QueueDeclare(queue: routingKey, durable: false, exclusive: false, autoDelete: true, arguments: null);

            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);
            _channel.BasicPublish(exchange: "",
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }
    }
}