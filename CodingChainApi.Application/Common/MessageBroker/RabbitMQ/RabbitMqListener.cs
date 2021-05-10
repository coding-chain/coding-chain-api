using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Application.Common.MessageBroker.RabbitMQ
{
    public class RabbitMqListener : IHostedService
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        protected string RouteKey;
        protected string QueueName;

        public RabbitMqListener(IOptions<MessageBrokerConfiguration> options)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = options.Value.RabbitHost
                };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitListener init error,ex:{ex.Message}");
            }
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register();
            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.connection.Close();
            return Task.CompletedTask;
        }

        // Registered consumer monitoring here
        public void Register()
        {
            try
            {
                Console.WriteLine($"RabbitListener register,routeKey:{RouteKey}");
                channel.ExchangeDeclare(exchange: "message", type: "topic");
                channel.QueueDeclare(queue: QueueName, exclusive: false);
                channel.QueueBind(queue: QueueName,
                    exchange: "message",
                    routingKey: RouteKey);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    var result = Process(message);
                    if (result)
                    {
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                };
                channel.BasicConsume(queue: QueueName, consumer: consumer);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"RabbitMQ error:{exception.Message}");
            }
        }

        public void DeRegister()
        {
            this.connection.Close();
        }

        // How to process messages
        public virtual bool Process(string message)
        {
            throw new NotImplementedException();
        }
    }
}