using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.Application.DTOs.Infrastructure;
using KwikNesta.Gateway.Svc.Application.Interfaces;
using KwikNesta.Gateway.Svc.Application.Queries.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KwikNesta.Gateway.Svc.API.Controllers.V1
{
    [Route("api/v{version:apiversion}/audits")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class AuditsController : ApiControllerBase
    {
        private readonly IInfrastructureServiceClient _infrastructure;

        public AuditsController(IInfrastructureServiceClient infrastructure)
        {
            _infrastructure = infrastructure;
        }

        /// <summary>
        /// Gets paginated list of audit logs
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<Paginator<AuditTrailDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAudits([FromQuery] GetAuditsQuery query)
        {
            return FromApiResponse(await _infrastructure.GetAuditsV1(query));
        }
    }
}