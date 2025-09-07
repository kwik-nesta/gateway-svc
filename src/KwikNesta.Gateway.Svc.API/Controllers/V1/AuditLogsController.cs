using KwikNesta.Gateway.Svc.API.DTOs;
using KwikNesta.Gateway.Svc.API.Extensions;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;
using KwikNesta.SystemSupport.Svc.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KwikNesta.Gateway.Svc.API.Controllers.V1
{
    [Route("api/v{version:apiversion}/audit-logs")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IServiceManager _service;

        public AuditLogsController(IServiceManager service)
        {
            _service = service;
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
        [HttpGet]
        [ProducesResponseType(typeof(GetAuditLogsListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RunLocationDataload([FromQuery] AuditLogPageQuery pageQuery)
        {
            var response = await _service.AuditLogs.GetAuditLogsAsync(pageQuery, HttpContext.GetHeaderMeta());
            return Ok(response);
        }
    }
}
