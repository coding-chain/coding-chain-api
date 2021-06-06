using System;
using Application.Write.ParticipationsSessions;
using CodingChainApi.Infrastructure.MessageBroker;
using CodingChainApi.Infrastructure.Services.Processes;
using CodingChainApi.Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeosCodingApi.Messaging
{
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