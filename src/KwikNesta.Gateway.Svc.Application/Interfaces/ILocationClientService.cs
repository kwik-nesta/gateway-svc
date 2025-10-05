using KwikNesta.Contracts.Models.Location;
using Refit;

namespace KwikNesta.Gateway.Svc.Application.Interfaces
{
    public interface ILocationClientService
    {
        [Get("/api/v1/location/countries")]
        Task<ApiResponse<List<KwikNestaCountry>>> GetCountriesAsyncV1();

        [Get("/api/v1/location/country/{countryId}/states")]
        Task<ApiResponse<List<KwikNestaState>>> GetStatesForCountryAsyncV1(Guid countryId);

        [Get("/api/v1/location/country/{countryId}/state/{stateId}/cities")]
        Task<ApiResponse<List<KwikNestaCity>>> GetCitiesForStateAsyncV1(Guid countryId,
                                                                        Guid stateId);
    }
}