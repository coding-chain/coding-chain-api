using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodingChainApi.Infrastructure.Common.Exceptions;
using CodingChainApi.Infrastructure.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NeosCodingApi.Messaging
{
    public abstract class RabbitMqBaseListener : IHostedService
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        protected readonly ILogger<RabbitMqBaseListener> Logger;
        protected string? Exchange;
        protected string? RoutingKey;

        public RabbitMqBaseListener(IRabbitMqSettings settings, ILogger<RabbitMqBaseListener> logger)
        {
            Logger = logger;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = settings.Host
                };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                Logger.LogError("RabbitListener init error,ex:{Error}", ex.Message);
                throw new InfrastructureException(ex.Message);
            }
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Register();
            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.Close();
            return Task.CompletedTask;
        }

        // Registered consumer monitoring here
        public void Register()
        {
            try
            {
                _channel.ExchangeDeclare(Exchange, ExchangeType.Topic);
                var queueName = _channel.QueueDeclare().QueueName;
                Logger.LogInformation(
                    "RabbitListener register, exchange: {Exchange}, routeKey:{RoutingKey}, queueName {QueueName}",
                    Exchange, RoutingKey, queueName);

                _channel.QueueBind(queueName,
                    Exchange,
                    RoutingKey);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());

                    var result = Process(message);
                    if (result) _channel.BasicAck(ea.DeliveryTag, false);
                };
                _channel.BasicConsume(queue: queueName, consumer: consumer);
            }
            catch (Exception exception)
            {
                Logger.LogError("RabbitMQ error:{Error}", exception.Message);
            }
        }

        public void DeRegister()
        {
            _connection.Close();
        }

        // How to process messages
        public abstract bool Process(string message);
    }
}