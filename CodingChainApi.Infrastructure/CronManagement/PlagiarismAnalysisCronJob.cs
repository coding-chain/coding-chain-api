using System;
using Microsoft.Extensions.Logging;
using Quartz;
using Application.Read.Plagiarism.Handlers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CodingChainApi.Infrastructure.CronManagement
{
    [DisallowConcurrentExecution]
    public class PlagiarismAnalysisCronJob : CronJob
    {
        private readonly ILogger<PlagiarismAnalysisCronJob> _logger;

        public PlagiarismAnalysisCronJob(ILogger<PlagiarismAnalysisCronJob> logger, IServiceProvider serviceProvider) :
            base(logger, serviceProvider)
        {
            _logger = logger;
        }


        protected override async void Process()
        {
            var mediator = GetScope().ServiceProvider.GetRequiredService<IMediator>();
            var functionsToCompare = await mediator.Send(new GetFunctionsToCompareRequest());
            
            _logger.LogInformation("oui");
        }
    }
}