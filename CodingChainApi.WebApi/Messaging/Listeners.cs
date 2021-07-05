using System;
using Application.Write.ParticipationsSessions;
using Application.Write.Plagiarism;
using CodingChainApi.Infrastructure.Settings;
using Microsoft.Extensions.Logging;

namespace CodingChainApi.Messaging
{
    public class PlagiarismExecutionDoneListener : NotifierListenerService<PlagiarismDoneAnalyzeNotification>
    {
        public PlagiarismExecutionDoneListener(IRabbitMqSettings settings,
            ILogger<PlagiarismExecutionDoneListener> logger, IServiceProvider serviceProvider) : base(settings, logger,
            serviceProvider)
        {
            Exchange = settings.PlagiarismExchange;
            RoutingKey = settings.PlagiarismAnalyzeDoneRoutingKey;
        }
    }

    public class ParticipationPreparedListener : NotifierListenerService<SetParticipationReadyStateNotification>
    {
        public ParticipationPreparedListener(IRabbitMqSettings settings,
            ILogger<ParticipationPreparedListener> logger, IServiceProvider serviceProvider) : base(settings, logger,
            serviceProvider)
        {
            Exchange = settings.ParticipationExchange;
            RoutingKey = settings.PreparedExecutionRoutingKey;
        }
    }

    public class ParticipationDoneExecutionListener : NotifierListenerService<ProcessEndNotification>
    {
        public ParticipationDoneExecutionListener(IRabbitMqSettings settings,
            ILogger<ParticipationDoneExecutionListener> logger, IServiceProvider serviceProvider) : base(settings,
            logger, serviceProvider)
        {
            Exchange = settings.ParticipationExchange;
            RoutingKey = settings.DoneExecutionRoutingKey;
        }
    }
}