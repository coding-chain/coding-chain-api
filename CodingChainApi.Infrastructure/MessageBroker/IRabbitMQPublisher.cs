namespace CodingChainApi.Infrastructure.MessageBroker
{
    public interface IRabbitMqPublisher
    {
        void PushMessage(string queueName, object message);
    }
}