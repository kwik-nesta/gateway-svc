using System.Text.Json.Serialization;

namespace KwikNesta.Gateway.Svc.API.DTOs
{
    public class ChangeForgotPasswordDto
    {
        public string Otp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
        [JsonIgnore]
        public bool IsValid => !string.IsNullOrWhiteSpace(Otp) && 
            !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(NewPassword) &&
            NewPassword == ConfirmNewPassword;
    }
}
