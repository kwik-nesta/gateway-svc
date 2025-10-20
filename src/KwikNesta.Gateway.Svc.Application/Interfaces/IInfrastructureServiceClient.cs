using KwikNesta.Contracts.DTOs;
using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.Application.DTOs.Infrastructure;
using KwikNesta.Gateway.Svc.Application.Queries.Infrastructure;
using Refit;

namespace KwikNesta.Gateway.Svc.Application.Interfaces
{
    public interface IInfrastructureServiceClient
    {
        #region Audit Endpoints
        [Get("/api/v1/audit-trails")]
        Task<ApiResponse<ApiResult<Paginator<AuditTrailDto>>>> GetAuditsV1([Query] GetAuditsQuery query);
        #endregion

        #region Location Endpoints
        /// <summary>
        /// Gets list of countries
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Get("/api/v1/locations/countries")]
        Task<ApiResponse<ApiResult<Paginator<CountryDto>>>> GetCountriesV1([Query] GetPagedCountriesQuery query);

        /// <summary>
        /// Gets all the states in a given country
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [Get("/api/v1/locations/countries/{countryId}/states")]
        Task<ApiResponse<ApiResult<List<StateDto>>>> GetCountryStatesV1(Guid countryId);

        /// <summary>
        /// Gets all the cities in a given state of a country
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [Get("/api/v1/locations/countries/{countryId}/states/{stateId}")]
        Task<ApiResponse<ApiResult<List<CityDto>>>> GetStateCitiesV1(Guid countryId, Guid stateId);
        #endregion
    }
}