using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.Application.Commands.Identity;
using KwikNesta.Gateway.Svc.Application.DTOs.Identity;
using Refit;

namespace KwikNesta.Gateway.Svc.Application.Interfaces
{
    public interface IIdentityServiceClient
    {
        private const string ApiBaseV1 = "/api/v1/auth";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Post($"/api/v1/auth/sign-in")]
        Task<ApiResponse<ApiResult<LoginResponseDto>>> LoginAsyncV1([Body] LoginCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Post($"/api/v1/auth/refresh-token")]
        Task<ApiResponse<ApiResult<RefreshTokenResponseDto>>> RefreshTokenV1([Body] RefreshTokenCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Post($"/api/v1/auth/sign-up")]
        Task<ApiResponse<ApiResult<RegisterResponseDto>>> RegisterAsyncV1([Body] RegisterCommand command);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Put($"/api/v1/auth/confirm")]
        Task<ApiResponse<ApiResult<string>>> VerifyAccountV1([Body] VerificationCommand command);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Post($"/api/v1/auth/request-otp")]
        Task<ApiResponse<ApiResult<string>>> ResendOtpV1([Body] ResendOtpCommand command);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Patch($"/api/v1/auth/reset-password")]
        Task<ApiResponse<ApiResult<string>>> PasswordResetV1([Body] PasswordResetCommand command);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Patch($"/api/v1/auth/change-forgot-password")]
        Task<ApiResponse<ApiResult<string>>> ChangeForgotPasswordV1([Body] ChangeForgotPasswordCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Patch($"/api/v1/auth/change-password")]
        Task<ApiResponse<ApiResult<string>>> ChangePasswordV1([Body] ChangePasswordCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Put($"/api/v1/auth/deactivate-account")]
        Task<ApiResponse<ApiResult<string>>> DeactivateV1(string userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Patch($"/api/v1/auth/reactivate-account")]
        Task<ApiResponse<ApiResult<string>>> Reactivate([Body] ReactivationCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Patch($"/api/v1/auth/request-reactivation")]
        Task<ApiResponse<ApiResult<string>>> RequestReactivationV1([Body] ReactivationRequestCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Patch("/api/v1/auth/lift-account-suspension/{userId}")]
        Task<ApiResponse<ApiResult<string>>> LiftSuspensionV1(string userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Put($"/api/v1/auth/suspend-account")]
        Task<ApiResponse<ApiResult<string>>> SuspendV1([Body] SuspendUserCommand command);
    }
}