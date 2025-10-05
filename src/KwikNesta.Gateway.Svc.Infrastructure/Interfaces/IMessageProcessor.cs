using Hangfire.Server;
using KwikNesta.Contracts.Models;

namespace KwikNesta.Gateway.Svc.Infrastructure.Interfaces
{
    public interface IMessageProcessor
    {
        Task HandleAsync(AuditLog message, PerformContext context);
        Task HandleAsync(NotificationMessage message, PerformContext context);
        Task HandleAsync(DataLoadRequest message, PerformContext context);
    }
}
