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
        private readonly ILogger _logger;
        private CronId CronId { get; set; }

        public CronJob(ILogger<CronJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected IServiceScope GetScope()
        {
            return _serviceProvider.CreateScope();
        }

        public void BeforeProcess(IJobExecutionContext context)
        {
            CronId = new CronId(GetScope().ServiceProvider.GetRequiredService<IMediator>()
                .Send(new CronRegisteredRequest(context.JobDetail.Key.Name, DateTime.Now)).Result);
        }

        public async void AfterProcess(CronStatusEnum newStatus)
        {
            await GetScope().ServiceProvider.GetRequiredService<IMediator>()
                .Send(new UpdateCronHandlerRequest(CronId.Value, newStatus));
        }

        protected abstract void Process();

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                BeforeProcess(context);
                Process();
                AfterProcess(CronStatusEnum.Success);

                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"Error running cron {context.JobDetail.Key.Name} : {exception.Message}");
                AfterProcess(CronStatusEnum.Error);
                return Task.FromCanceled(new CancellationToken(true));
            }
        }
    }
}