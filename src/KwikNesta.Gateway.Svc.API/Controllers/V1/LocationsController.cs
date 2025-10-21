using KwikNesta.Contracts.DTOs;
using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.Application.Interfaces;
using KwikNesta.Gateway.Svc.Application.Queries.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace KwikNesta.Gateway.Svc.API.Controllers.V1
{
    [Route("api/v{version:apiversion}/locations")]
    [ApiVersion("1.0")]
    [ApiController]
    public class LocationsController : ApiControllerBase
    {
        private readonly IInfrastructureServiceClient _infrastructure;

        public LocationsController(IInfrastructureServiceClient infrastructure)
        {
            _infrastructure = infrastructure;
        }

        /// <summary>
        /// Gets a list of paginated countries
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("countries")]
        [ProducesResponseType(typeof(ApiResult<Paginator<CountryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries([FromQuery] GetPagedCountriesQuery query)
        {
            return FromApiResponse(await _infrastructure.GetCountriesV1(query));
        }

        /// <summary>
        /// Get a list of states by country id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet("countries/{countryId}/states")]
        [ProducesResponseType(typeof(ApiResult<List<StateDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStates([FromRoute] Guid countryId)
            => FromApiResponse(await _infrastructure.GetCountryStatesV1(countryId));

        /// <summary>
        /// Get all cities by country and state ids
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpGet("countries/{countryId}/states/{stateId}")]
        [ProducesResponseType(typeof(ApiResult<List<CityDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCities([FromRoute] Guid countryId, [FromRoute] Guid stateId)
            => FromApiResponse(await _infrastructure.GetStateCitiesV1(countryId, stateId));
    }
}