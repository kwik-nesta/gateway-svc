using KwikNesta.Gateway.Svc.API.Grpc.SystemSupport;

namespace KwikNesta.Gateway.Svc.API.Services.Interfaces
{
    public interface IGrpcLocationService
    {
        Task<CitiesResponse> GetCitiesByStateAndCountry(string stateId, string countryId, PagedRequest query);
        Task<CityResponse> GetCityById(string id);
        Task<CountryResponse> GetCountry(string id);
        Task<CountriesResponse> GetPagedCountries(PagedRequest request);
        Task<StateResponse> GetStateById(string id);
        Task<StatesResponse> GetStatesByCountry(string countryId, PagedRequest query);
    }
}
