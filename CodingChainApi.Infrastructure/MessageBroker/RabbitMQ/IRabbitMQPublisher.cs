namespace Application.Common.MessageBroker.RabbitMQ
{
    public interface IRabbitMQPublisher
    {
        void PushMessage(string routingKey, object message);
    }
}