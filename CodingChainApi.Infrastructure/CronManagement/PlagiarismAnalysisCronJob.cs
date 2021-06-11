using System;
using System.Threading.Tasks;
using Application.Read.Cron.Handlers;
using Microsoft.Extensions.Logging;
using Quartz;
using Application.Read.Plagiarism.Handlers;
using Application.Write.Plagiarism;
using Domain.Cron;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CodingChainApi.Infrastructure.CronManagement
{
    [DisallowConcurrentExecution]
    public class PlagiarismAnalysisCronJob : CronJob
    {
        private DateTime? LastExecutionDate { get; set; }

        public PlagiarismAnalysisCronJob(ILogger<PlagiarismAnalysisCronJob> logger, IServiceProvider serviceProvider) :
            base(logger, serviceProvider)
        {
        }

        public override async Task BeforeProcess(IJobExecutionContext context)
        {
            using var scope = GetScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await base.BeforeProcess(context);
            LastExecutionDate =
                await mediator.Send(new GetLastCronExecutionRequest(context.JobDetail.Key.Name,
                    CronStatusEnum.Success));
        }

        protected override async void Process()
        {
            using var scope = GetScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var functionsToCompare = await mediator.Send(new GetFunctionsToCompareRequest());
            var suspectedFunctions = await mediator.Send(new GetSuspectedFunctionRequest(LastExecutionDate));
            foreach (var function in suspectedFunctions)
            {
                await mediator.Send(new CodePlagiarismPendingExecutionRequest(function, functionsToCompare));
            }
            
        }
    }
}