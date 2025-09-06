using KwikNesta.Gateway.Svc.API.DTOs;
using KwikNesta.Gateway.Svc.API.Extensions;
using KwikNesta.Gateway.Svc.API.Grpc.Identity;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KwikNesta.Gateway.Svc.API.Controllers.V1
{
    [Route("api/v{version:apiversion}/user")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IServiceManager _service;

        public UserController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves the currently authenticated user's profile information.
        /// </summary>
        /// <remarks>
        /// Requires a valid JWT bearer token in the request header.
        /// </remarks>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the authenticated user's details
        /// if the request is authorized; otherwise, an unauthorized response.
        /// </returns>
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetLoggedInUser()
        {
            var headers = HttpContext.GetHeaderMeta();
            var user = await _service.User.GetAuthenticatedUser(headers);
            return Ok(user);
        }

        /// <summary>
        /// Retrieves a paginated list of users based on the provided query parameters.
        /// </summary>
        /// <param name="query">The query parameters for pagination, filtering, or sorting of users.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the paginated list of users.
        /// </returns>
        /// <remarks>
        /// Accessible only to users with the <c>Admin</c> or <c>SuperAdmin</c> role.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetPagedUsers([FromQuery] UserQuery query)
        {
            var meta = HttpContext.GetHeaderMeta();
            var pagedData = await _service.User.GetPagedUsers(query, meta);
            return Ok(pagedData);
        }

        /// <summary>
        /// Retrieves a single user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the user details if found.
        /// </returns>
        /// <remarks>
        /// Requires the caller to be authenticated.
        /// </remarks>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            var user = await _service.User.GetUserById(id, HttpContext.GetHeaderMeta());
            return Ok(user);
        }
    }
}