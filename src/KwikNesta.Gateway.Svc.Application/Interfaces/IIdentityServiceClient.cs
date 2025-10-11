using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.Application.Commands.Identity;
using KwikNesta.Gateway.Svc.Application.DTOs.Identity;
using KwikNesta.Gateway.Svc.Application.Queries.Identity;
using Refit;
using System.Data;

namespace KwikNesta.Gateway.Svc.Application.Interfaces
{
    public interface IIdentityServiceClient
    {
        #region Auth Endpoints
        /// <summary>
        /// Authenticates user
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
        /// Registers new user
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Post($"/api/v1/auth/sign-up")]
        Task<ApiResponse<ApiResult<RegisterResponseDto>>> RegisterAsyncV1([Body] RegisterCommand command);
        
        /// <summary>
        /// Verifies user account
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Put($"/api/v1/auth/confirm")]
        Task<ApiResponse<ApiResult<string>>> VerifyAccountV1([Body] VerificationCommand command);
        
        /// <summary>
        /// Resends OTP
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Post($"/api/v1/auth/request-otp")]
        Task<ApiResponse<ApiResult<string>>> ResendOtpV1([Body] ResendOtpCommand command);
        
        /// <summary>
        /// Requests password reset
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Patch($"/api/v1/auth/reset-password")]
        Task<ApiResponse<ApiResult<string>>> PasswordResetV1([Body] PasswordResetCommand command);
        
        /// <summary>
        /// Change forgotten user's password
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Patch($"/api/v1/auth/change-forgot-password")]
        Task<ApiResponse<ApiResult<string>>> ChangeForgotPasswordV1([Body] ChangeForgotPasswordCommand command);

        /// <summary>
        /// Changes user password
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Patch($"/api/v1/auth/change-password")]
        Task<ApiResponse<ApiResult<string>>> ChangePasswordV1([Body] ChangePasswordCommand command);

        /// <summary>
        /// Deactivates user account
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Put("/api/v1/auth/deactivate-account/{userId}")]
        Task<ApiResponse<ApiResult<string>>> DeactivateV1(string userId);

        /// <summary>
        /// Reactivates user account
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Patch($"/api/v1/auth/reactivate-account")]
        Task<ApiResponse<ApiResult<string>>> Reactivate([Body] ReactivationCommand command);

        /// <summary>
        /// Request account reactivation
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Patch($"/api/v1/auth/request-reactivation")]
        Task<ApiResponse<ApiResult<string>>> RequestReactivationV1([Body] ReactivationRequestCommand command);

        /// <summary>
        /// Lifts user suspension
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Patch("/api/v1/auth/lift-account-suspension/{userId}")]
        Task<ApiResponse<ApiResult<string>>> LiftSuspensionV1(string userId);

        /// <summary>
        /// Suspends a user
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Put($"/api/v1/auth/suspend-account")]
        Task<ApiResponse<ApiResult<string>>> SuspendV1([Body] SuspendUserCommand command);
        #endregion

        #region User Endpoints
        /// <summary>
        /// Gets current logged in user details
        /// </summary>
        /// <returns></returns>
        [Get("/api/v1/user/current")]
        Task<ApiResponse<ApiResult<CurrentUserDto>>> GetCurrentUserV1();

        /// <summary>
        /// Updates user's basic information
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Patch("/api/v1/user/update")]
        Task<ApiResponse<ApiResult<CurrentUserDto>>> UpdateUserBasicInfoV2([Body] UpdateBasicUserDetailsCommand command);

        /// <summary>
        /// Gets user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Get("/api/v1/user/{id}")]
        Task<ApiResponse<ApiResult<CurrentUserDto>>> GetUserById(string id);

        /// <summary>
        /// Gets list of users by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Post("/api/v1/user")]
        Task<ApiResponse<ApiResult<List<CurrentUserDto>>>> GetUsersByIds([Body] List<string> ids);

        /// <summary>
        /// Gets paged users
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Get("/api/v1/user")]
        Task<ApiResponse<ApiResult<PagedUsersResponseDto>>> GetPaged([Query] GetPagedUsersQuery query);
        #endregion
    }
}