using KwikNesta.Gateway.Svc.API.Extensions;
using KwikNesta.Gateway.Svc.API.Grpc.SystemSupport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KwikNesta.Gateway.Svc.API.Controllers.V1
{
    [Route("api/v{version:apiversion}/dataload")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class DataloadController : ControllerBase
    {
        private readonly DataloadService.DataloadServiceClient _serviceClient;

        public DataloadController(DataloadService.DataloadServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        /// <summary>
        /// Executes the location data load process.
        /// </summary>
        /// <remarks>
        /// This endpoint triggers a background process that loads or refreshes location-related data 
        /// (such as countries, states, and cities) into the system.  
        /// It does not require a request body.
        /// </remarks>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the response of the data load operation.
        /// </returns>
        [HttpPost("location")]
        [ProducesResponseType(typeof(DataloadStringResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RunLocationDataload()
        {
            var response = await _serviceClient
                .RunLocationDataloadAsync(new DataloadEmpty { },
                    HttpContext.GetHeaderMeta());

            return Ok(response);
        }
    }
}
