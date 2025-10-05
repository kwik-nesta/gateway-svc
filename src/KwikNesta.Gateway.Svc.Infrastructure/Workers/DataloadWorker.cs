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
    public class DataloadWorker : BackgroundService
    {
        private readonly ILoggerManager _logger;
        private readonly IRabbitMQPubSub _pubSub;

        public DataloadWorker(ILoggerManager logger,
                                  IRabbitMQPubSub pubSub)
        {
            _logger = logger;
            _pubSub = pubSub;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo("Dataload Worker started....");

            _pubSub.Subscribe<DataLoadRequest>(MQs.DataLoad.GetDescription(), async msg =>
            {
                _logger.LogInfo("Received data-load request.\nType: {Type}. \nRequest By: {RequesterEmail}.\nDate: {Date}",
                    msg.Type.GetDescription(), msg.RequesterEmail, msg.Date);

                BackgroundJob.Enqueue<IMessageProcessor>(pr => pr.HandleAsync(msg, null!));
                await Task.CompletedTask;

            }, routingKey: MQRoutingKey.DataLoad.GetDescription());

            await Task.CompletedTask;
        }
    }
}
