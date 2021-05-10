namespace CodingChainApi.Infrastructure.Settings
{
    public interface IRabbitMQSettings
    {
        string RabbitHost { get; set; }
        string RabbitUserName { get; set; }
        string RabbitPassword { get; set; }
        int RabbitPort { get; set; }
        string ExecutionCodeRoute { get; set; }
        string RabbitMqWorker { get; set; }
        string RoutingKey { get; set; }
    }
}