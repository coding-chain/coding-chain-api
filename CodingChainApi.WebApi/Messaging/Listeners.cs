using Application.Write.ParticipationsSessions;
using Application.Write.Plagiarism;
using CodingChainApi.Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Logging;

namespace NeosCodingApi.Messaging
{
    public class PlagiarismExecutionDoneListener : NotifierListenerService<PlagiarismDoneAnalyzeNotification>
    {
        public PlagiarismExecutionDoneListener(IRabbitMqSettings settings,
            ILogger<PlagiarismExecutionDoneListener> logger, IPublisher mediator) : base(settings, logger, mediator)
        {
            Exchange = settings.PlagiarismExchange;
            RoutingKey = settings.PlagiarismAnalyzeDoneRoutingKey;
        }
    }

    public class ParticipationPreparedListener : NotifierListenerService<SetParticipationReadyStateNotification>
    {
        public ParticipationPreparedListener(IRabbitMqSettings settings,
            ILogger<ParticipationPreparedListener> logger, IPublisher mediator) : base(settings, logger, mediator)
        {
            Exchange = settings.ParticipationExchange;
            RoutingKey = settings.PreparedExecutionRoutingKey;
        }
    }

    public class ParticipationDoneExecutionListener : NotifierListenerService<ProcessEndNotification>
    {
        public ParticipationDoneExecutionListener(IRabbitMqSettings settings,
            ILogger<ParticipationDoneExecutionListener> logger, IPublisher mediator) : base(settings, logger, mediator)
        {
            Exchange = settings.ParticipationExchange;
            RoutingKey = settings.DoneExecutionRoutingKey;
        }
    }
}