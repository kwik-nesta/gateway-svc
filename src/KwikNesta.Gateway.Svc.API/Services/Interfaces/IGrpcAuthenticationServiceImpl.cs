using Grpc.Core;
using KwikNesta.Gateway.Svc.API.DTOs;
using KwikNesta.Gateway.Svc.API.Grpc.Identity;

namespace KwikNesta.Gateway.Svc.API.Services.Interfaces
{
    public interface IGrpcAuthenticationServiceImpl
    {
        Task<StringResponse> ChangeForgotPasswordAsync(ChangeForgotPasswordDto request);
        Task<StringResponse> ChangePasswordAsync(ChangePasswordRequest request, Metadata meta);
        Task<StringResponse> DeactivateAsync(DeactivateAccountRequest request, Metadata meta);
        Task<StringResponse> ListSuspensionAsync(LiftUserSuspensionRequest request, Metadata meta);
        Task<TokenResponse> LoginAsync(LoginRequest request);
        Task<StringResponse> ReactivateAsync(ReactivateAccountRequest request);
        Task<TokenResponse> RefreshAccessTokenAsync(RefreshTokenRequest request);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<StringResponse> RequestPasswordResetAsync(EmailRequest request);
        Task<StringResponse> RequestReactivationAsync(RequestAccountReactivationRequest request);
        Task<StringResponse> ResendOtpAsync(ResendOtpRequest request);
        Task<StringResponse> SuspendAsync(SuspendUserRequest request, Metadata meta);
        Task<StringResponse> VerifyAsync(OtpRequest request);
    }
}
