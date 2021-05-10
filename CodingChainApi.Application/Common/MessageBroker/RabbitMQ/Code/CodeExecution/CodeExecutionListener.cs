using System;
using Application.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Application.Common.MessageBroker.RabbitMQ.Code.CodeExecution
{
    public class CodeExecutionListener : RabbitMqListener
    {
        private readonly ILogger<RabbitMqListener> _logger;
        private readonly IServiceProvider _services;

        public CodeExecutionListener(IServiceProvider services, IOptions<MessageBrokerConfiguration> options,
            ILogger<RabbitMqListener> logger) : base(options)
        {
            base.RouteKey = "code.execution.done";
            base.QueueName = "coding.chain.worker";
            _services = services;
            _logger = logger;
        }

        public override bool Process(string message)
        {
            var taskMessage = JToken.Parse(message);
            if (taskMessage == null)
            {
                // When false is returned, the message is rejected directly, indicating that it cannot be processed
                return false;
            }

            try
            {
                using (var scope = _services.CreateScope())
                {
                    //var xxxService = scope.ServiceProvider.GetRequiredService<XXXXService>();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Process fail,error:{ex.Message},stackTrace:{ex.StackTrace},message:{message}");
                _logger.LogError(-1, ex, "Process fail");
                return false;
            }
        }
    }
}