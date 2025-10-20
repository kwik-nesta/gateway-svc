namespace KwikNesta.Gateway.Svc.Application.DTOs
{
    public class AuditTrailDto
    {
        public string Domain { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
