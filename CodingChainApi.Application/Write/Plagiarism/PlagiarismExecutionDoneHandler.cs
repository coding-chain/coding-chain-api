using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.CodeAnalysis;
using Domain.Participations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Write.Plagiarism
{
    public record FunctionSimilarity(Guid Id, double Rate, string Hash);

    public record PlagiarismDoneAnalyzeNotification
        (Guid SuspectedFunctionId, string SuspectHash, IList<FunctionSimilarity> ComparedFunctions) : INotification;

    public class PlagiarismExecutionDoneHandler : INotificationHandler<PlagiarismDoneAnalyzeNotification>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ITimeService _timeService;

        public PlagiarismExecutionDoneHandler(
            ITimeService timeService, IServiceProvider serviceProvider)
        {
            _timeService = timeService;
            this.serviceProvider = serviceProvider;
        }

        public async Task Handle(PlagiarismDoneAnalyzeNotification notification, CancellationToken cancellationToken)
        {
            var plagiarizedFunctionRepository = serviceProvider.CreateScope().ServiceProvider
                .GetRequiredService<ISuspectFunctionRepository>();
            var functionId = new FunctionId(notification.SuspectedFunctionId);

            var plagiarizedFunctions = notification.ComparedFunctions.Select(func =>
                new PlagiarizedFunctionEntity(new FunctionId(func.Id), func.Rate, func.Hash,notification.SuspectHash, _timeService.Now())).ToList();
            var suspectFunction = await plagiarizedFunctionRepository.FindByIdAsync(functionId) ??
                                  SuspectFunctionAggregate.CreateNew(functionId,notification.SuspectHash, new List<PlagiarizedFunctionEntity>());
            suspectFunction.SetPlagiarizedFunctions(plagiarizedFunctions);
            await plagiarizedFunctionRepository.SetAsync(suspectFunction);
        }
    }
}