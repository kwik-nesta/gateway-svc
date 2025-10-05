using KwikNesta.Contracts.Enums;

namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public record UpdateBasicUserDetailsCommand
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? OtherName { get; set; }
        public Gender Gender { get; set; }
    }
}
