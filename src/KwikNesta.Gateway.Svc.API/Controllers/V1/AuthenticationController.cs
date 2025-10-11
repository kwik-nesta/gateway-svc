using Elasticsearch.Net;
using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.API.Grpc.Identity;
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
        private readonly IIdentityServiceClient _identityService;

        public AuthenticationController(IIdentityServiceClient identityService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Authenticates a user with the provided credentials and issues access/refresh tokens.
        /// </summary>
        /// <param name="request"> The login request payload containing email and password
        /// </param>
        /// <returns>
        /// A<see cref = "LoginResponseDto" /> containing access and refresh tokens
        /// </returns>
        /// <response code="200">Login successful</response>
        /// <response code="400">Invalid request.</response>
        /// <response code="500">An unexpected error occurred while processing the registration.</response>
        [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        {
            return FromApiResponse(await _identityService.LoginAsyncV1(request));
        }

        /// <summary>
        /// Registers a new user account within the system.
        /// </summary>
        /// <param name="command">The registration request payload containing the user’s personal, contact, and authentication details.
        /// <br/><br/>
        /// <b>Accepted values for Role: </b>
        /// <list type="bullet">
        /// <item><description><c>1</c> = Super Admin, </description></item>
        /// <item><description><c>2</c> = Admin, </description></item>
        /// <item><description><c>3</c> = Landlord, </description></item>
        /// <item><description><c>4</c> = Tenant, </description></item>
        /// <item><description><c>5</c> = Agent</description></item>
        /// </list>
        /// <br/><br/>
        /// <b>Accepted values for Gender: </b>
        /// <list type="bullet">
        /// <item><description><c>0</c> = Others, </description></item>
        /// <item><description><c>1</c> = Male, </description></item>
        /// <item><description><c>2</c> = Female, </description></item>
        /// </list>
        /// <br/><br/>
        /// <b>Password and ConfirmPassword: </b>
        /// <list type="bullet">
        /// <item><description>Must match exactly.</description></item>
        /// </list>
        /// <br/><br/>
        /// <b>Phone number: </b>
        /// <list type="bullet">
        /// <item><description>Must be in international format: +12345...</description></item>
        /// </list>
        ///  <br/><br/>
        /// </param>
        /// <returns>
        /// A <see cref="RegisterResponseDto"/> containing details of the newly created account or any validation errors encountered.
        /// </returns>
        /// <response code="200">Registration was successful and a new user account has been created.</response>
        /// <response code="400">One or more fields in the registration data are invalid or incomplete.</response>
        /// <response code="401">Unauthorized attempt to register (e.g., insufficient permissions).</response>
        /// <response code="500">An unexpected error occurred while processing the registration.</response>
        [ProducesResponseType(typeof(ApiResult<RegisterResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            return FromApiResponse(await _identityService.RegisterAsyncV1(command));
        }


        /// <summary>
        /// Verifies a user's account using an OTP (One-Time Password).
        /// </summary>
        /// <param name="command">The verification request containing the OTP and email address.</param>
        /// <returns>A <see cref="ApiResult{TResult}"/> with verification status.</returns>
        /// <response code="200">Verification successful.</response>
        /// <response code="400">Invalid OTP or request.</response>
        /// <response code="500">Internal server error.</response>
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPatch("verify")]
        public async Task<IActionResult> Verify([FromBody] VerificationCommand command)
        {
            return FromApiResponse(await _identityService.VerifyAccountV1(command));
        }

        /// <summary>
        /// Resends the OTP (One-Time Password) for account verification.
        /// </summary>
        /// <param name="command">The resend OTP request.
        /// <br/><br/>
        /// <b>Accepted values for OTP Type: </b>
        /// <list type="bullet">
        /// <item><description><c>0</c> = Account Verification</description></item>
        /// <item><description><c>1</c> = Password Reset</description></item>
        /// <item><description><c>2</c> = Account Reactivation</description></item>
        /// </list>
        /// <br/><br/>
        /// </param>
        /// <returns>A <see cref="ApiResult{TResult}"/> indicating resend status.</returns>
        /// <response code="200">OTP resent successfully.</response>
        /// <response code="400">Invalid request.</response>
        /// <response code="500">Internal server error.</response>
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
        {
            var response = await _identityService.ResendOtpV1(command);
            return FromApiResponse(response);
        }

        /// <summary>
        /// Refreshes a user's access token using a valid refresh token.
        /// </summary>
        /// <param name="request">The refresh token request.</param>
        /// <returns>A <see cref="ApiResult{TResult}"/> containing the new access token.</returns>
        /// <response code="200">Token refreshed successfully.</response>
        /// <response code="400">Invalid refresh token provided.</response>
        /// <response code="500">Internal server error.</response>
        [ProducesResponseType(typeof(ApiResult<RefreshTokenResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand request)
        {
            var response = await _identityService.RefreshTokenV1(request);
            return FromApiResponse(response);
        }

        /// <summary>
        /// Initiates the password reset process by sending a reset OTP to the user.
        /// </summary>
        /// <param name="request">The email request containing the user's email address.</param>
        /// <returns>A <see cref="ApiResult{TResult}"/> with reset initiation status.</returns>
        /// <response code="200">Password reset request successful.</response>
        /// <response code="400">Invalid email provided.</response>
        /// <response code="500">Internal server error.</response>
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPatch("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetCommand request)
        {
            var response = await _identityService.PasswordResetV1(request);
            return FromApiResponse(response);
        }

        /// <summary>
        /// Changes the forgotten password using a valid reset token/OTP.
        /// </summary>
        /// <param name="request">The change-forgot-password request.
        /// <br/><br/>
        /// <b>NewPassword and ConfirmNewPassword: </b>
        /// <list type="bullet">
        /// <item><description>Must match exactly.</description></item>
        /// </list>
        /// <br/><br/>
        /// </param>
        /// <returns>A <see cref="ApiResult{TResult}"/> with change password operation status.</returns>
        /// <response code="200">If the password was successfully changed.</response>
        /// <response code="400">If the reset token is invalid or expired.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPatch("change-forgot-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeForgotPasswordCommand request)
        {
            var response = await _identityService.ChangeForgotPasswordV1(request);
            return FromApiResponse(response);
        }

        /// <summary>
        /// Changes the current user's password.
        /// </summary>
        /// <param name="request">The change password request containing old and new passwords.
        /// <br/><br/>
        /// <b>NewPassword and ConfirmNewPassword: </b>
        /// <list type="bullet">
        /// <item><description>Must match exactly.</description></item>
        /// </list>
        /// <br/><br/>
        /// </param>
        /// <returns>A <see cref="ApiResult{TResult}"/> with result status.</returns>
        /// <response code="200">If the password was changed successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPatch("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand request)
        {
            var response = await _identityService.ChangePasswordV1(request);
            return FromApiResponse(response);
        }

        /// <summary>
        /// Deactivates a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to deactivate.</param>
        /// <returns>A <see cref="ApiResult{TResult}"/> with deactivation result.</returns>
        /// <response code="200">If the account was deactivated successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPatch("deactivate-account/{userId}")]
        [Authorize]
        public async Task<IActionResult> Deactivate([FromRoute] string userId)
        {
            var response = await _identityService.DeactivateV1(userId);
            return FromApiResponse(response);
        }

        /// <summary>
        /// Reactivates a deactivated account using OTP verification.
        /// </summary>
        /// <param name="request">The OTP reactivation request.</param>
        /// <returns>A <see cref="ApiResult{TResult}"/> with reactivation status.</returns>
        /// <response code="200">If the account was reactivated successfully.</response>
        /// <response code="400">If the OTP is invalid or expired.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPatch("reactivate-account")]
        public async Task<IActionResult> Reactivate([FromBody] ReactivationCommand request)
        {
            var response = await _identityService.Reactivate(request);
            return FromApiResponse(response);
        }

        /// <summary>
        /// Requests reactivation for a previously deactivated account.
        /// </summary>
        /// <param name="request">The email request identifying the account.</param>
        /// <returns>A <see cref="ApiResult{TResult}"/> with request status.</returns>
        /// <response code="200">If the reactivation request was submitted successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPatch("reactivate-account-request")]
        public async Task<IActionResult> ReactivateRequest([FromBody] ReactivationRequestCommand request)
        {
            var response = await _identityService.RequestReactivationV1(request);
            return FromApiResponse(response);
        }

        /// <summary>
        /// Suspends a user account (admin roles only).
        /// </summary>
        /// <param name="request">The suspension request.
        /// <br/><br/>
        /// <b>Accepted Values for Suspension Reason: </b>
        /// <list type="bullet">
        /// <item><description><c>0</c> = Suspicious Activity, </description></item>
        /// <item><description><c>1</c> = Compromised Account, </description></item>
        /// <item><description><c>2</c> = Policy Violation</description></item>
        /// <item><description><c>3</c> = Fraudulent Behavior, </description></item>
        /// <item><description><c>4</c> = Payment Failure, </description></item>
        /// <item><description><c>5</c> = Pending Investigation, </description></item>
        /// <item><description><c>6</c> = Duplicate Account, </description></item>
        /// <item><description><c>7</c> = User Reports, </description></item>
        /// <item><description><c>8</c> = System Abuse, </description></item>
        /// <item><description><c>9</c> = Other</description></item>
        /// </list>
        /// <br/><br/>
        /// </param>
        /// <returns>A <see cref="ApiResult{TResult}"/> with suspension result.</returns>
        /// <response code="200">If the account was suspended successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPatch("suspend-account")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Suspend([FromBody] SuspendUserCommand request)
        {
            var response = await _identityService.SuspendV1(request);
            return FromApiResponse(response);
        }

        /// <summary>
        /// Lifts users' suspended accounts (admin roles only).
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        [HttpPatch("lift-suspension/{userId}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> LiftSuspension([FromRoute] string userId)
        {
            var response = await _identityService.LiftSuspensionV1(userId);
            return FromApiResponse(response);
        }
    }
}