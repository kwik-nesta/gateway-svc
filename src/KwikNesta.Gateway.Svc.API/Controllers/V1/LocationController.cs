using KwikNesta.Gateway.Svc.API.DTOs;
using KwikNesta.Gateway.Svc.API.Grpc.SystemSupport;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KwikNesta.Gateway.Svc.API.Controllers.V1
{
    [Route("api/v{version:apiversion}/location")]
    [ApiVersion("1.0")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IServiceManager _service;

        public LocationController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a paginated list of countries.
        /// </summary>
        /// <param name="query">Pagination and filtering parameters for the country list.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the paginated list of countries.
        /// </returns>
        [HttpGet("countries")]
        [ProducesResponseType(typeof(CountriesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries([FromQuery] PagedDataQuery query)
        {
            var data = await _service.Location.GetPagedCountries(new PagedRequest
            {
                Search = query.Search,
                Page = query.Page,
                Size = query.PageSize
            });
            return Ok(data);
        }

        /// <summary>
        /// Retrieves a specific country by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the country.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the country details if found.
        /// </returns>
        [HttpGet("countries/{id}")]
        [ProducesResponseType(typeof(CountryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry([FromRoute] string id)
        {
            var pagedData = await _service.Location.GetCountry(id);
            return Ok(pagedData);
        }

        /// <summary>
        /// Retrieves a paginated list of states within a specified country.
        /// </summary>
        /// <param name="countryId">The unique identifier of the country.</param>
        /// <param name="query">Pagination and filtering parameters for the state list.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the paginated list of states.
        /// </returns>
        [HttpGet("countries/{countryId}/states")]
        [ProducesResponseType(typeof(StatesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStates([FromRoute] string countryId, [FromQuery] PagedDataQuery query)
        {
            var pagedData = await _service.Location.GetStatesByCountry(countryId, new PagedRequest
            {
                Search = query.Search,
                Page = query.Page,
                Size = query.PageSize
            });
            return Ok(pagedData);
        }

        /// <summary>
        /// Retrieves a specific state by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the state.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the state details if found.
        /// </returns>
        [HttpGet("states/{id}")]
        [ProducesResponseType(typeof(StateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetState([FromRoute] string id)
        {
            var data = await _service.Location.GetStateById(id);
            return Ok(data);
        }

        /// <summary>
        /// Retrieves a paginated list of cities within a specified state and country.
        /// </summary>
        /// <param name="countryId">The unique identifier of the country.</param>
        /// <param name="stateId">The unique identifier of the state.</param>
        /// <param name="query">Pagination and filtering parameters for the city list.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the paginated list of cities.
        /// </returns>
        [HttpGet("countries/{countryId}/states/{stateId}/cities")]
        [ProducesResponseType(typeof(CitiesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCities([FromRoute] string countryId,
                                                   [FromRoute] string stateId,
                                                   [FromQuery] PagedDataQuery query)
        {
            var pagedData = await _service.Location.GetCitiesByStateAndCountry(stateId, countryId, new PagedRequest
            {
                Search = query.Search,
                Page = query.Page,
                Size = query.PageSize
            });
            return Ok(pagedData);
        }

        /// <summary>
        /// Retrieves a specific city by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the city.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the city details if found.
        /// </returns>
        [HttpGet("cities/{id}")]
        [ProducesResponseType(typeof(CityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCities([FromRoute] string id)
        {
            var pagedData = await _service.Location.GetCityById(id);
            return Ok(pagedData);
        }
    }
}