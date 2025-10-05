namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public record RefreshTokenCommand
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
