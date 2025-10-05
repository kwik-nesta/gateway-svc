using CrossQueue.Hub.Services.Interfaces;
using CSharpTypes.Extensions.Enumeration;
using DiagnosKit.Core.Logging.Contracts;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;
using Microsoft.Extensions.Hosting;

namespace KwikNesta.Gateway.Svc.Infrastructure.Workers
{
    public class AuditWorker : BackgroundService
    {
        private readonly ILoggerManager _logger;
        private readonly IRabbitMQPubSub _pubSub;

        public AuditWorker(ILoggerManager logger,
                                  IRabbitMQPubSub pubSub)
        {
            _logger = logger;
            _pubSub = pubSub;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo("Audit Worker started....");

            _pubSub.Subscribe<AuditLog>(MQs.Audit.GetDescription(), async msg =>
            {
                _logger.LogInfo("Received audit for action: {Action}", msg.Action.GetDescription());
                //BackgroundJob.Enqueue<IMessageProcessor>(pr => pr.MethodName);

            }, routingKey: MQRoutingKey.AuditTrails.GetDescription());

            await Task.CompletedTask;
        }
    }
}
