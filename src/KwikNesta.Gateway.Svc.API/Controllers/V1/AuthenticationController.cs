using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.API.DTOs;
using KwikNesta.Gateway.Svc.API.Extensions;
using KwikNesta.Gateway.Svc.API.Grpc.Identity;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;
using KwikNesta.Gateway.Svc.Application.Commands.Identity;
using KwikNesta.Gateway.Svc.Application.DTOs.Identity;
using KwikNesta.Gateway.Svc.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KwikNesta.Gateway.Svc.API.Controllers.V1
{
    [Route("api/v{version:apiversion}/auth")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthenticationController : ApiControllerBase
    {
        private readonly IServiceManager _service;
        private readonly IIdentityServiceClient _identityService;

        public AuthenticationController(IServiceManager service, IIdentityServiceClient identityService)
        {
            _service = service;
            _identityService = identityService;
        }

        /// <summary>
        /// Authenticates a user with the provided credentials and issues access/refresh tokens.
        /// </summary>
        /// <param name="request">
        /// The <see cref="LoginCommand"/> containing the user's login credentials
        /// (e.g., username/email and password).
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing:
        /// <list type="bullet">
        ///   <item>
        ///     <description><see cref="OkObjectResult"/> (200) with a <see cref="LoginResponseDto"/> 
        ///     if authentication is successful.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="UnauthorizedResult"/> (401) if credentials are invalid.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="BadRequestResult"/> (400) if the request payload is malformed.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="StatusCodeResult"/> (500) if an unexpected error occurs on the server.</description>
        ///   </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This endpoint should be consumed over HTTPS only.  
        /// Example request payload:
        /// <code>
        /// {
        ///   "username": "user@example.com",
        ///   "password": "P@ssw0rd!"
        /// }
        /// </code>
        /// Example response payload (200 OK):
        /// <code>
        /// {
        ///   "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///   "refreshToken": "d4c7a4e2-bf6f-4f1a-a1a0-2f6c5a0eddb1",
        ///   "expiresIn": 3600
        /// }
        /// </code>
        /// </remarks>
        [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        {
            return FromApiResponse(await _identityService.LoginAsyncV1(request));
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="request">The registration details including email, password, and other required info.</param>
        /// <returns>A <see cref="RegisterResponse"/> indicating the result of the registration process.</returns>
        /// <response code="200">Registration successful.</response>
        /// <response code="400">Invalid registration data provided.</response>
        /// <response code="401">Unauthorized attempt to register.</response>
        /// <response code="500">Internal server error.</response>
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _service.Authentication.RegisterAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Verifies a user's account using an OTP (One-Time Password).
        /// </summary>
        /// <param name="request">The verification request containing the OTP.</param>
        /// <returns>A <see cref="StringResponse"/> with verification status.</returns>
        /// <response code="200">Verification successful.</response>
        /// <response code="400">Invalid OTP or request.</response>
        /// <response code="500">Internal server error.</response>
        [ProducesResponseType(typeof(StringResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("verify")]
        public async Task<IActionResult> Verify([FromBody] OtpRequest request)
        {
            var response = await _service.Authentication.VerifyAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Resends the OTP (One-Time Password) for account verification.
        /// </summary>
        /// <param name="request">The resend OTP request.</param>
        /// <returns>A <see cref="StringResponse"/> indicating resend status.</returns>
        /// <response code="200">OTP resent successfully.</response>
        /// <response code="400">Invalid request.</response>
        /// <response code="500">Internal server error.</response>
        [ProducesResponseType(typeof(StringResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            var response = await _service.Authentication.ResendOtpAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Refreshes a user's access token using a valid refresh token.
        /// </summary>
        /// <param name="request">The refresh token request.</param>
        /// <returns>A <see cref="TokenResponse"/> containing the new access token.</returns>
        /// <response code="200">Token refreshed successfully.</response>
        /// <response code="400">Invalid refresh token provided.</response>
        /// <response code="500">Internal server error.</response>
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _service.Authentication.RefreshAccessTokenAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Initiates the password reset process by sending a reset link or OTP to the user.
        /// </summary>
        /// <param name="request">The email request containing the user's email address.</param>
        /// <returns>A <see cref="StringResponse"/> with reset initiation status.</returns>
        /// <response code="200">Password reset request successful.</response>
        /// <response code="400">Invalid email provided.</response>
        /// <response code="500">Internal server error.</response>
        [ProducesResponseType(typeof(StringResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] EmailRequest request)
        {
            var response = await _service.Authentication.RequestPasswordResetAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Changes the forgotten password using a valid reset token/OTP.
        /// </summary>
        /// <param name="request">The change-forgot-password request.</param>
        /// <returns>A <see cref="StringResponse"/> with operation status.</returns>
        /// <response code="200">If the password was successfully changed.</response>
        /// <response code="400">If the reset token is invalid or expired.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(StringResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("change-forgot-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeForgotPasswordDto request)
        {
            var response = await _service.Authentication.ChangeForgotPasswordAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Changes the current user's password.
        /// </summary>
        /// <param name="request">The change password request containing old and new passwords.</param>
        /// <returns>A <see cref="StringResponse"/> with result status.</returns>
        /// <response code="200">If the password was changed successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(StringResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var meta = HttpContext.GetHeaderMeta();
            var response = await _service.Authentication.ChangePasswordAsync(request, meta);
            return Ok(response);
        }

        /// <summary>
        /// Deactivates a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to deactivate.</param>
        /// <returns>A <see cref="StringResponse"/> with deactivation result.</returns>
        /// <response code="200">If the account was deactivated successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(StringResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("deactivate-account/{userId}")]
        [Authorize]
        public async Task<IActionResult> Deactivate([FromRoute] string userId)
        {
            var meta = HttpContext.GetHeaderMeta();

            var response = await _service.Authentication.DeactivateAsync(new DeactivateAccountRequest
            {
                Request = new UserIdRequest
                {
                    UserId = userId
                }
            }, meta);
            return Ok(response);
        }

        /// <summary>
        /// Reactivates a deactivated account using OTP verification.
        /// </summary>
        /// <param name="request">The OTP reactivation request.</param>
        /// <returns>A <see cref="StringResponse"/> with reactivation status.</returns>
        /// <response code="200">If the account was reactivated successfully.</response>
        /// <response code="400">If the OTP is invalid or expired.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(StringResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("reactivate-account")]
        public async Task<IActionResult> Reactivate([FromBody] OtpRequest request)
        {
            var response = await _service.Authentication.ReactivateAsync(new ReactivateAccountRequest
            {
                Request = request,
            });
            return Ok(response);
        }

        /// <summary>
        /// Requests reactivation for a previously deactivated account.
        /// </summary>
        /// <param name="request">The email request identifying the account.</param>
        /// <returns>A <see cref="StringResponse"/> with request status.</returns>
        /// <response code="200">If the reactivation request was submitted successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(StringResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("reactivate-account-request")]
        public async Task<IActionResult> ReactivateRequest([FromBody] EmailRequest request)
        {
            var response = await _service.Authentication.RequestReactivationAsync(new RequestAccountReactivationRequest
            {
                Request = request
            });
            return Ok(response);
        }

        /// <summary>
        /// Suspends a user account (admin only).
        /// </summary>
        /// <param name="request">The suspension request.</param>
        /// <returns>A <see cref="StringResponse"/> with suspension result.</returns>
        /// <response code="200">If the account was suspended successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(StringResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("suspend-account")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Suspend([FromBody] SuspendUserRequest request)
        {
            var meta = HttpContext.GetHeaderMeta();

            var response = await _service.Authentication.SuspendAsync(request, meta);
            return Ok(response);
        }

        // <summary>
        /// Lifts a suspension from a user account (admin only).
        /// </summary>
        /// <param name="userId">The ID of the user to lift the suspension for.</param>
        /// <returns>A <see cref="StringResponse"/> with suspension removal status.</returns>
        /// <response code="200">If the suspension was lifted successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(StringResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("lift-suspension/{userId}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> LiftSuspension([FromRoute] string userId)
        {
            var meta = HttpContext.GetHeaderMeta();

            var response = await _service.Authentication.ListSuspensionAsync(new LiftUserSuspensionRequest
            {
                Request = new UserIdRequest
                {
                    UserId = userId
                }
            }, meta);
            return Ok(response);
        }
    }
}