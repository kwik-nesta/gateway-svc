using KwikNesta.Contracts.Enums;

namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public record ResendOtpCommand
    {
        public string Email { get; set; } = string.Empty;
        public OtpType Type { get; set; }
    }
}
