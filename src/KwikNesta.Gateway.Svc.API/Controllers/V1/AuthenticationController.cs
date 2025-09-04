using KwikNesta.Gateway.Svc.API.Grpc.Identity;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KwikNesta.Gateway.Svc.API.Controllers.V1
{
    [Route("api/v{version:apiversion}/auth")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceManager _service;

        public AuthenticationController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Authenticates a user with the provided credentials and issues access/refresh tokens.
        /// </summary>
        /// <param name="request">
        /// The <see cref="LoginRequest"/> containing the user's login credentials
        /// (e.g., username/email and password).
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing:
        /// <list type="bullet">
        ///   <item>
        ///     <description><see cref="OkObjectResult"/> (200) with a <see cref="TokenResponse"/> 
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
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var tokens = await _service.Authentication.LoginAsync(request);
            return Ok(tokens);
        }
    }
}
