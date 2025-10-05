namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public record ChangePasswordCommand
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
