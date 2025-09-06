using KwikNesta.Gateway.Svc.API.Grpc.SystemSupport;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;

namespace KwikNesta.Gateway.Svc.API.Services
{
    public class GrpcLocationServiceImpl : IGrpcLocationService
    {
        private readonly LocationService.LocationServiceClient _serviceClient;

        public GrpcLocationServiceImpl(LocationService.LocationServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public async Task<CountriesResponse> GetPagedCountries(PagedRequest request)
        {
            var pagedData = await _serviceClient.GetCountriesAsync(request);
            return pagedData;
        }

        public async Task<CountryResponse> GetCountry(string id)
        {
            var country = await _serviceClient.GetCountryByIdAsync(new IdRequest
            {
                Id = id
            });

            return country;
        }

        public async Task<StatesResponse> GetStatesByCountry(string countryId,
                                                             PagedRequest query)
        {
            var statesResponse = await _serviceClient.GetStatesByCountryAsync(new CountryIdAndPagedRequest
            {
                CountryId = countryId,
                Query = query
            });

            return statesResponse;
        }

        public async Task<StateResponse> GetStateById(string id)
        {
            var stateResponse = await _serviceClient.GetStateByIdAsync(new IdRequest
            {
                Id = id
            });

            return stateResponse;
        }

        public async Task<CitiesResponse> GetCitiesByStateAndCountry(string stateId,
                                                                     string countryId,
                                                                     PagedRequest query)
        {
            var cities = await _serviceClient.GetCitiesByCountryAndStateAsync(new CountryAndStateIdsRequest
            {
                StateId = stateId,
                CountryId = countryId,
                Query = query
            });

            return cities;
        }

        public async Task<CityResponse> GetCityById(string id)
        {
            var city = await _serviceClient.GetCityByIdAsync(new IdRequest
            {
                Id = id
            });

            return city;
        }
    }
}