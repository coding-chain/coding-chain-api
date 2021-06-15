using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Write.Cron.Handlers.RegisterCron;
using Application.Write.Cron.Handlers.UpdateCron;
using Domain.Cron;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Microsoft.Extensions.Logging;

namespace CodingChainApi.Infrastructure.CronManagement
{
    public abstract class CronJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        protected readonly ILogger<CronJob> Logger;
        private CronId CronId { get; set; }

        public CronJob(ILogger<CronJob> logger, IServiceProvider serviceProvider)
        {
            Logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected IServiceScope GetScope()
        {
            return _serviceProvider.CreateScope();
        }

        public virtual async Task BeforeProcess(IJobExecutionContext context)
        {
            using var scope = GetScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var id = await mediator.Send(new CronRegisteredRequest(context.JobDetail.Key.Name));
            CronId = new CronId(id);
        }

        public virtual async Task AfterProcess(CronStatusEnum newStatus)
        {
            using var scope = GetScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new UpdateCronHandlerRequest(CronId.Value, newStatus));
        }

        protected abstract Task Process();

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.LogInformation("Before cron job name : {JobName} execution triggered", context.JobDetail.Key.Name);
                await BeforeProcess(context);
                Logger.LogInformation("Before cron job name : {JobName} execution done", context.JobDetail.Key.Name);
                
                Logger.LogInformation("Process cron job name : {JobName} execution triggered", context.JobDetail.Key.Name);
                Process();
                Logger.LogInformation("Process cron job name : {JobName} execution triggered", context.JobDetail.Key.Name);
                
                Logger.LogInformation("After cron job name : {JobName} execution triggered", context.JobDetail.Key.Name);
                await AfterProcess(CronStatusEnum.Success);
                Logger.LogInformation("After cron job name : {JobName} execution triggered", context.JobDetail.Key.Name);
            }
            catch (Exception exception)
            {
                Logger.LogError("Error running cron {CronName} : {Error}", context.JobDetail.Key.Name,
                    exception.Message);
                // return Task.FromCanceled(new CancellationToken(true));
            }
        }
    }
}