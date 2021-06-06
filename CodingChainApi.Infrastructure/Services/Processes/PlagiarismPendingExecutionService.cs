using Application.Contracts.IService;
using CodingChainApi.Infrastructure.MessageBroker;
using CodingChainApi.Infrastructure.Settings;
using Microsoft.Extensions.Logging;

namespace CodingChainApi.Infrastructure.Services.Processes
{
    public class PlagiarismPendingExecutionService : RabbitMqBasePublisher, IPlagiarismPendingExecutionService
    {
        public PlagiarismPendingExecutionService(IRabbitMqSettings settings,
            ILogger<PlagiarismPendingExecutionService> logger) : base(settings, logger)
        {
            Exchange = settings.PlagiarismExchange;
            RoutingKey = settings.PlagiarismAnalyzeExecutionRoutingKey;
        }
        public void StartExecution(PlagiarismAnalyzeExecutionDto command)
        {
            PushMessage(command);
        }
    }
}