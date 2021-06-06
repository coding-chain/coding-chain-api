using System;
using System.Collections.Generic;
using Application.Common.Exceptions;
using Application.Contracts.IService;
using Application.Contracts.Processes;
using Application.Write.ParticipationsSessions;
using CodingChainApi.Infrastructure.MessageBroker;
using CodingChainApi.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodingChainApi.Infrastructure.Services.Processes
{

    public class ParticipationPendingExecutionService : RabbitMqBasePublisher, IParticipationPendingExecutionService
    {
        public ParticipationPendingExecutionService(IRabbitMqSettings settings,
            ILogger<ParticipationPendingExecutionService> logger) : base(settings, logger)
        {
            Exchange = settings.ParticipationExchange;
            RoutingKey = settings.PendingExecutionRoutingKey;
        }


        public void StartExecution(RunParticipationTestsDto command)
        {
            PushMessage(command);
        }
    }
}