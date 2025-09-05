using KwikNesta.Gateway.Svc.API.DTOs;
using KwikNesta.Gateway.Svc.API.Grpc.Identity;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;

namespace KwikNesta.Gateway.Svc.API.Services
{
    public class GrpcAuthenticationServiceImpl : IGrpcAuthenticationServiceImpl
    {
        private readonly AuthenticationService.AuthenticationServiceClient _service;

        public GrpcAuthenticationServiceImpl(AuthenticationService.AuthenticationServiceClient service) 
            => _service = service;

        /// <summary>
        /// Authenticates a user and issues a new access/refresh token pair.
        /// </summary>
        /// <param name="request">The login request containing user credentials.</param>
        /// <returns>A <see cref="TokenResponse"/> with issued tokens.</returns>
        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            var loginResult = await _service.LoginAsync(request);
            return loginResult.Tokens;
        }

        /// <summary>
        /// Refreshes an expired access token using a valid refresh token.
        /// </summary>
        /// <param name="request">The refresh token request.</param>
        /// <returns>A <see cref="TokenResponse"/> with a new access token.</returns>
        public async Task<TokenResponse> RefreshAccessTokenAsync(RefreshTokenRequest request)
        {
            var otpResponse = await _service.RefreshAsync(request);
            return otpResponse.Token;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="request">The registration details.</param>
        /// <returns>A <see cref="RegisterResponse"/> with account creation results.</returns>
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            return await _service.RegisterAsync(request);
        }

        /// <summary>
        /// Verifies a newly registered account using an OTP or token.
        /// </summary>
        /// <param name="request">The verification request.</param>
        /// <returns>A <see cref="StringResponse"/> containing verification status.</returns>
        public async Task<StringResponse> VerifyAsync(OtpRequest request)
        {
            var verifyResponse = await _service.VerifyAccountAsync(new VerifyAccountRequest
            {
                Request = request
            });
            return verifyResponse.Response;
        }

        /// <summary>
        /// Resends a one-time password (OTP) for account verification.
        /// </summary>
        /// <param name="request">The resend OTP request.</param>
        /// <returns>A <see cref="StringResponse"/> containing resend status.</returns>
        public async Task<StringResponse> ResendOtpAsync(ResendOtpRequest request)
        {
            var otpResponse = await _service.ResendOtpAsync(request);
            return otpResponse.Response;
        }

        /// <summary>
        /// Initiates a password reset flow for a user account.
        /// </summary>
        /// <param name="request">The password reset request.</param>
        /// <returns>A <see cref="StringResponse"/> with reset initiation status.</returns>
        public async Task<StringResponse> RequestPasswordResetAsync(EmailRequest request)
        {
            var otpResponse = await _service.PasswordResetAsync(new PasswordResetRequest
            {
                Request = request
            });
            return otpResponse.Response;
        }

        /// <summary>
        /// Completes a forgotten password reset by setting a new password.
        /// </summary>
        /// <param name="request">The change-forgot-password request.</param>
        /// <returns>A <see cref="StringResponse"/> with operation result.</returns>
        public async Task<StringResponse> ChangeForgotPasswordAsync(ChangeForgotPasswordDto request)
        {
            if (!request.IsValid)
            {
                return new StringResponse
                {
                    Status = 400,
                    Message = "One or more validation check failed. Please check your inputs and try again"
                };
            }

            var otpResponse = await _service.ChangeForgotPasswordAsync(new ChangeForgotPasswordRequest
            {
                Request = new OtpRequest
                {
                    Otp = request.Otp,
                    Email = request.Email
                },
                NewPassword = request.NewPassword,
                ConfirmPassword = request.ConfirmNewPassword
            });
            return otpResponse.Response;
        }

        /// <summary>
        /// Changes the current user password using the old password for validation.
        /// </summary>
        /// <param name="request">The change password request.</param>
        /// <returns>A <see cref="StringResponse"/> with operation result.</returns>
        public async Task<StringResponse> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var otpResponse = await _service.ChangePasswordAsync(request);
            return otpResponse.Response;
        }

        /// <summary>
        /// Deactivates a user account.
        /// </summary>
        /// <param name="request">The deactivation request.</param>
        /// <returns>A <see cref="StringResponse"/> with deactivation result.</returns>
        public async Task<StringResponse> DeactivateAsync(DeactivateAccountRequest request)
        {
            var otpResponse = await _service.DeactivateAccountAsync(request);
            return otpResponse.Response;
        }

        /// <summary>
        /// Reactivates a previously deactivated account.
        /// </summary>
        /// <param name="request">The reactivation request.</param>
        /// <returns>A <see cref="StringResponse"/> with reactivation result.</returns>
        public async Task<StringResponse> ReactivateAsync(ReactivateAccountRequest request)
        {
            var otpResponse = await _service.ReactivateAccountAsync(request);
            return otpResponse.Response;
        }

        /// <summary>
        /// Requests reactivation for a deactivated account.
        /// </summary>
        /// <param name="request">The reactivation request details.</param>
        /// <returns>A <see cref="StringResponse"/> with request status.</returns>
        public async Task<StringResponse> RequestReactivationAsync(RequestAccountReactivationRequest request)
        {
            var otpResponse = await _service.RequestAccountReactivationAsync(request);
            return otpResponse.Response;
        }

        /// <summary>
        /// Suspends a user account (e.g., for policy violations).
        /// </summary>
        /// <param name="request">The suspension request.</param>
        /// <returns>A <see cref="StringResponse"/> with suspension result.</returns>
        public async Task<StringResponse> SuspendAsync(SuspendUserRequest request)
        {
            var otpResponse = await _service.SuspendUserAsync(request);
            return otpResponse.Response;
        }

        /// <summary>
        /// Lifts a suspension on a user account.
        /// </summary>
        /// <param name="request">The lift suspension request.</param>
        /// <returns>A <see cref="StringResponse"/> with result of suspension removal.</returns>
        public async Task<StringResponse> ListSuspensionAsync(LiftUserSuspensionRequest request)
        {
            var otpResponse = await _service.LiftUserSuspensionAsync(request);
            return otpResponse.Response;
        }
    }
}
