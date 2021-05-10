using System;
using System.Text;
using Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Application.Common.MessageBroker.RabbitMQ
{
    public class RabbitMQPublisher
    {
        private readonly IModel _channel;

        private readonly ILogger _logger;

        public RabbitMQPublisher(IOptions<MessageBrokerConfiguration> options, ILogger<RabbitMQPublisher> logger)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = options.Value.RabbitHost
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
            _channel.QueueDeclare(queue: "message",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);
            _channel.BasicPublish(exchange: "message",
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }
    }
}