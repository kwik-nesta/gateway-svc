namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public record ReactivationRequestCommand
    {
        public string Email { get; set; } = string.Empty;
    }
}
