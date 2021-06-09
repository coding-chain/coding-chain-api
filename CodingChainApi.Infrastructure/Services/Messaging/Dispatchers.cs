using Application.Contracts.Dtos;
using CodingChainApi.Infrastructure.Settings;
using Microsoft.Extensions.Logging;

namespace CodingChainApi.Infrastructure.Services.Messaging
{
    public class ParticipationPendingExecutionService : BaseDispatcherService<RunParticipationTestsDto>
    {
        public ParticipationPendingExecutionService(IRabbitMqSettings settings,
            ILogger<ParticipationPendingExecutionService> logger) : base(settings, logger)
        {
            Exchange = settings.ParticipationExchange;
            RoutingKey = settings.PendingExecutionRoutingKey;
        }
    }

    public class PlagiarismPendingExecutionService : BaseDispatcherService<PlagiarismAnalyzeExecutionDto>
    {
        public PlagiarismPendingExecutionService(IRabbitMqSettings settings,
            ILogger<PlagiarismPendingExecutionService> logger) : base(settings, logger)
        {
            Exchange = settings.PlagiarismExchange;
            RoutingKey = settings.PlagiarismAnalyzeExecutionRoutingKey;
        }
    }

    public class PrepareParticipationExecutionService : BaseDispatcherService<PrepareParticipationExecutionDto>
    {
        public PrepareParticipationExecutionService(IRabbitMqSettings settings,
            ILogger<PrepareParticipationExecutionService> logger) : base(settings, logger)
        {
            Exchange = settings.ParticipationExchange;
            RoutingKey = settings.PrepareExecutionRoutingKey;
        }
    }

    public class CleanParticipationExecutionService : BaseDispatcherService<CleanParticipationExecutionDto>
    {
        public CleanParticipationExecutionService(IRabbitMqSettings settings,
            ILogger<CleanParticipationExecutionService> logger) : base(settings, logger)
        {
            Exchange = settings.ParticipationExchange;
            RoutingKey = settings.CleanExecutionRoutingKey;
        }
    }
}