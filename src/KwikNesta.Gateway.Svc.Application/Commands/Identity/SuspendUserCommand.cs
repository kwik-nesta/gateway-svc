using KwikNesta.Contracts.Enums;
namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public record SuspendUserCommand
    {
        public string UserId { get; set; } = string.Empty;
        public SuspensionReasons Reason { get; set; }
    }
}