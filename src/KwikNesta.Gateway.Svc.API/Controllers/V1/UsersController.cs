using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.Application.Commands.Identity;
using KwikNesta.Gateway.Svc.Application.DTOs.Identity;
using KwikNesta.Gateway.Svc.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KwikNesta.Gateway.Svc.API.Controllers.V1
{
    [Route("api/v{version:apiversion}/users")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UsersController : ApiControllerBase
    {
        private readonly IIdentityServiceClient _identityService;

        public UsersController(IIdentityServiceClient identityService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Gets current user info
        /// </summary>
        /// <returns>
        /// A<see cref = "CurrentUserDto" /> object containing current logged in user info
        /// </returns>
        /// <response code="200">Success response</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="401">Expired token</response>
        /// <response code="500">An unexpected error occurred while processing the registration.</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ApiResult<CurrentUserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrent()
        {
            return FromApiResponse(await _identityService.GetCurrentUserV1());
        }

        /// <summary>
        /// Updates basic user details
        /// </summary>
        /// <param name="command"> Profile update payload
        /// <br/><br/>
        /// <b>Accepted values for Gender: </b>
        /// <list type="bullet">
        /// <item><description><c>0</c> = Others, </description></item>
        /// <item><description><c>1</c> = Male, </description></item>
        /// <item><description><c>2</c> = Female</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// A<see cref = "CurrentUserDto" /> object containing current logged in user info
        /// </returns>
        /// <response code="200">Success response</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="401">Expired token</response>
        /// <response code="500">An unexpected error occurred while processing the registration.</response>
        [HttpPatch]
        [Authorize]
        [ProducesResponseType(typeof(ApiResult<CurrentUserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBasic([FromBody] UpdateBasicUserDetailsCommand command)
        {
            return FromApiResponse(await _identityService.UpdateUserBasicInfoV2(command));
        }
    }
}