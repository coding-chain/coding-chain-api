using System;
using CodingChainApi.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodingChainApi.Infrastructure.MessageBroker.RabbitMQ.Code.CodeExecution
{
    /** public class CodeExecutionListener : RabbitMqBaseListener
     {
         private readonly ILogger<RabbitMqBaseListener> _logger;
         private readonly IServiceProvider _services;
 
         public CodeExecutionListener(IServiceProvider services, IRabbitMQSettings settings,
             ILogger<RabbitMqBaseListener> logger) : base(settings, logger)
         {
             base.RouteKey = settings.ExecutionCodeRoute;
             base.QueueName = settings.CodeRouteKey;
             _services = services;
             _logger = logger;
         }
 
         public override bool Process(string message)
         {
             var taskMessage = message;
             if (taskMessage == null)
             {
                 // When false is returned, the message is rejected directly, indicating that it cannot be processed
                 return false;
             }
 
             try
             {
                 using (var scope = _services.CreateScope())
                 {
                     _logger.LogDebug($"message: ${taskMessage}");
                     //var xxxService = scope.ServiceProvider.GetRequiredService<XXXXService>();
                     return true;
                 }
             }
             catch (Exception ex)
             {
                 _logger.LogError($"Process fail,error:{ex.Message},stackTrace:{ex.StackTrace},message:{message}");
                 _logger.LogError(-1, ex, "Process fail");
                 return false;
             }
         }
     } **/
}