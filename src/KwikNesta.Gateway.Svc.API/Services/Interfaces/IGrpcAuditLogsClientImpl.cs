using Grpc.Core;
using KwikNesta.Gateway.Svc.API.DTOs;

namespace KwikNesta.Gateway.Svc.API.Services.Interfaces
{
    public interface IGrpcAuditLogsClientImpl
    {
        Task<AuditLogsPagedResponse> GetAuditLogsAsync(AuditLogPageQuery query, Metadata meta);
    }
}
