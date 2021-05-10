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

namespace CodingChainApi.Infrastructure.MessageBroker.RabbitMQ
{
    public abstract class RabbitMqBaseListener<TImplementation> : IHostedService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        protected string RouteKey;
        protected string QueueName;
        protected string QueueWorker;
        protected readonly ILogger<TImplementation> _logger;

        public RabbitMqBaseListener(IRabbitMQSettings settings, ILogger<TImplementation> logger)
        {
            _logger = logger;
            RouteKey = settings.RoutingKey;
            QueueName = "code.execution.done";
            QueueWorker = settings.RabbitMqWorker;
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = settings.RabbitHost
                };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                _logger.LogError($"RabbitListener init error,ex:{ex.Message}");
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
            this._connection.Close();
            return Task.CompletedTask;
        }

        // Registered consumer monitoring here
        public void Register()
        {
            try
            {
                Console.WriteLine($"RabbitListener register,routeKey:{RouteKey}");
                _channel.ExchangeDeclare(exchange: QueueWorker, type: "topic");
                _channel.QueueDeclare(queue: QueueName, exclusive: false);
                _channel.QueueBind(queue: QueueName,
                    exchange: QueueWorker,
                    routingKey: RouteKey);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());

                    var result = Process(message);
                    if (result)
                    {
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                };
                _channel.BasicConsume(queue: QueueName, consumer: consumer);
            }
            catch (Exception exception)
            {
                _logger.LogError($"RabbitMQ error:{exception.Message}");
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