namespace CodingChainApi.Infrastructure.MessageBroker.RabbitMQ
{
    public interface IRabbitMqPublisher
    {
        void PushMessage(string queueName, object message);
    }
}