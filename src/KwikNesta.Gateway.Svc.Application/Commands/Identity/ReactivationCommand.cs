namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public record ReactivationCommand
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}
