using KwikNesta.SystemSupport.Svc.Contracts;

namespace KwikNesta.Gateway.Svc.API.DTOs
{
    public class AuditLogsResponse
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string PerformedOn { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
    }

    public class AuditLogsPagedResponse
    {
        public List<AuditLogsResponse> AuditLogs { get; set; } = [];
        public GrpcAuditLogPageMetaData? PageMetaData { get; set; }
    }
}
