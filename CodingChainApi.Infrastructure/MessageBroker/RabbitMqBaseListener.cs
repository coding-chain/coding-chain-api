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

namespace CodingChainApi.Infrastructure.MessageBroker
{
    public abstract class RabbitMqBaseListener<TImplementation> : IHostedService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        protected abstract string RouteKey { get; set; }
        protected abstract string QueueName { get; set; }
        private readonly string _queueWorker;
        protected readonly ILogger<RabbitMqBaseListener<TImplementation>> Logger;

        public RabbitMqBaseListener(IRabbitMQSettings settings, ILogger<RabbitMqBaseListener<TImplementation>> logger)
        {
            Logger = logger;
            _queueWorker = settings.RabbitMqWorker;
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
                Logger.LogError("RabbitListener init error,ex:{Message}",ex.Message);
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
                Logger.LogDebug("RabbitListener register,routeKey:{RouteKey}", RouteKey);
                _channel.ExchangeDeclare(exchange: _queueWorker, type: "topic");
                _channel.QueueDeclare(queue: QueueName, exclusive: false);
                _channel.QueueBind(queue: QueueName,
                    exchange: _queueWorker,
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
                Logger.LogError("RabbitMQ error:{ExceptionMessage}",exception.Message);
            }
        }


        // How to process messages
        public abstract bool Process(string message);
    }
}