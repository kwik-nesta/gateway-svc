namespace KwikNesta.Gateway.Svc.API.Settings
{
    public class KwikNestaServers
    {
        public string IdentityService { get; set; } = string.Empty;
        public string PaymentService { get; set; } = string.Empty;
        public string PropertyService { get; set; } = string.Empty;

        public string ExternalLocationClient { get; set; } = string.Empty;
    }
}
