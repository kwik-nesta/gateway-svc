namespace KwikNesta.Gateway.Svc.API.Settings
{
    public class JwtSetting
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string RoleClaim { get; set; } = string.Empty;
        public string IssuerSigningKey { get; set; } = string.Empty;
    }
}
