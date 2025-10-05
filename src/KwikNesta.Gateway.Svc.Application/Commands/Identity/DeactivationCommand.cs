namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public record DeactivationCommand
    {
        public string UserId { get; set; } = string.Empty;
    }
}
