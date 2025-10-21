namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public record PasswordResetCommand
    {
        public string Email { get; set; } = string.Empty;
    }
}