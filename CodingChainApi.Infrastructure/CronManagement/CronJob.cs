using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Write.Cron.Handlers.RegisterCron;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Microsoft.Extensions.Logging;

namespace CodingChainApi.Infrastructure.CronManagement
{
    public record CronEvent() : INotification
    {
        private string _name;
        private DateTime _executedAt;

        public CronEvent(string name, DateTime executedAt) : this()
        {
            _name = name;
            _executedAt = executedAt;
        }
    }

    public abstract class CronJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public CronJob(ILogger<CronJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected IServiceScope GetScope()
        {
            return _serviceProvider.CreateScope();
        }

        public void Register(IJobExecutionContext context)
        {
            GetScope().ServiceProvider.GetRequiredService<IMediator>()
                .Send(new CronRegisteredRequest(context.JobDetail.Key.Name, new DateTime()));
        }

        protected abstract void Process();

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                Process();
                Register(context);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"Error running cron {context.JobDetail.Key.Name} : {exception.Message}");
                return Task.FromCanceled(new CancellationToken(true));
            }
        }
    }
}