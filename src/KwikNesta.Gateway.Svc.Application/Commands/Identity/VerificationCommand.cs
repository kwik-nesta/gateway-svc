namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public class VerificationCommand
    {
        public string Otp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
