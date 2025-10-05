using Hangfire.Server;
using KwikNesta.Contracts.Models;

namespace KwikNesta.Gateway.Svc.Application.Interfaces
{
    public interface IMessageProcessor
    {
        Task HandleAsync(AuditLog message, PerformContext context);
        Task HandleAsync(NotificationMessage message, PerformContext context);
        Task HandleAsync(DataLoadRequest message, PerformContext context);
    }
}
