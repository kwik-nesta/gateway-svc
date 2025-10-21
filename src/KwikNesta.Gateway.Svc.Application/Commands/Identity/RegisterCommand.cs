using KwikNesta.Contracts.Enums;

namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public record RegisterCommand
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public SystemRoles Role { get; set; }
    }
}