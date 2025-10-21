namespace KwikNesta.Gateway.Svc.Application.Settings
{
    public class PingSettings
    {
        public string Interval { get; set; } = "*/5 * * * *";
        public List<string> ServiceUrls { get; set; } = [];
    }
}