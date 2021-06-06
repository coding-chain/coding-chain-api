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
}