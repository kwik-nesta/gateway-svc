namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public record LiftSuspensionCommand
    {
        public string UserId { get; set; } = string.Empty;
    }
}