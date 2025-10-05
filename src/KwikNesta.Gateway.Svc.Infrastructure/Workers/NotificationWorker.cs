using CrossQueue.Hub.Services.Interfaces;
using CSharpTypes.Extensions.Enumeration;
using DiagnosKit.Core.Logging.Contracts;
using Hangfire;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.Infrastructure.Interfaces;
using Microsoft.Extensions.Hosting;

namespace KwikNesta.Gateway.Svc.Infrastructure.Workers
{
    public class NotificationWorker : BackgroundService
    {
        private readonly ILoggerManager _logger;
        private readonly IRabbitMQPubSub _pubSub;

        public NotificationWorker(ILoggerManager logger, 
                                  IRabbitMQPubSub pubSub)
        {
            _logger = logger;
            _pubSub = pubSub;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo("Notification Worker started....");

            _pubSub.Subscribe<NotificationMessage>(MQs.Notification.GetDescription(), async msg =>
            {
                _logger.LogInfo("Received notification for {EmailAddress}", msg.EmailAddress);
                BackgroundJob.Enqueue<IMessageProcessor>(pr => pr.HandleAsync(msg, null!));
                await Task.CompletedTask;

            }, routingKey: MQRoutingKey.AccountEmail.GetDescription());

            await Task.CompletedTask;
        }
    }
}
