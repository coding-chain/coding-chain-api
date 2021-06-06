namespace CodingChainApi.Infrastructure.Settings
{
    public interface IRabbitMqSettings
    {
        string RabbitHost { get; set; }
        string RabbitUserName { get; set; }
        string RabbitPassword { get; set; }
        int RabbitPort { get; set; }
        string PendingExecutionRoutingKey { get; set; }
        string DoneExecutionRoutingKey { get; set; }
        string ParticipationExchange { get; set; }
        string PlagiarismExchange { get; set; }
        string PlagiarismAnalyzeExecutionRoutingKey { get; set; }
        string PlagiarismAnalyzeDoneRoutingKey { get; set; }
    }
    public class RabbitMqSettings :  IRabbitMqSettings
    {
        public string RabbitHost { get; set; }
        public string RabbitUserName { get; set; }
        public string RabbitPassword { get; set; }
        public int RabbitPort { get; set; }
        public string PendingExecutionRoutingKey { get; set; }
        public string DoneExecutionRoutingKey { get; set; }
        public string ParticipationExchange { get; set; }
        public string PlagiarismExchange { get; set; }
        public string PlagiarismAnalyzeExecutionRoutingKey { get; set; }
        public string PlagiarismAnalyzeDoneRoutingKey { get; set; }
    }
}