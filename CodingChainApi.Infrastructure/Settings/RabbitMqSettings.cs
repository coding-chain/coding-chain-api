namespace CodingChainApi.Infrastructure.Settings
{
    public interface IRabbitMqSettings
    {
        string Host { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        int Port { get; set; }
        string PendingExecutionRoutingKey { get; set; }
        string DoneExecutionRoutingKey { get; set; }
        string ParticipationExchange { get; set; }
        string PlagiarismExchange { get; set; }
        string PlagiarismAnalyzeExecutionRoutingKey { get; set; }
        string PlagiarismAnalyzeDoneRoutingKey { get; set; }
        string PrepareExecutionRoutingKey { get; set; }
        string CleanExecutionRoutingKey { get; set; }
        string PreparedExecutionRoutingKey { get; set; }
    }

    public class RabbitMqSettings : IRabbitMqSettings
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string PendingExecutionRoutingKey { get; set; }
        public string DoneExecutionRoutingKey { get; set; }
        public string ParticipationExchange { get; set; }
        public string PlagiarismExchange { get; set; }
        public string PlagiarismAnalyzeExecutionRoutingKey { get; set; }
        public string PlagiarismAnalyzeDoneRoutingKey { get; set; }
        public string PrepareExecutionRoutingKey { get; set; }
        public string CleanExecutionRoutingKey { get; set; }
        public string PreparedExecutionRoutingKey { get; set; }
    }
}