using System.Configuration;
using RestaurantsNearMe.Services.Contracts;
using RestaurantsNearMe.Services.Models;

namespace RestaurantsNearMe.Services.Implementations
{
    public class RestaurantService : IRestaurantService
    {
        private IRestClientWrapper _restClientWrapper;
        private IClientConfiguration _clientConfiguration;
        private IConfiguration _configuration;

        public RestaurantService(IRestClientWrapper restClientWrapper, IClientConfiguration clientConfiguration, IConfiguration configuration)
        {
            _restClientWrapper = restClientWrapper;
            _clientConfiguration = clientConfiguration;
            _configuration = configuration;
        }

        public string GetAllRestaurants(string searchCode)
        {
            var fullApiUrl = string.Format(_configuration.RestaurantsResource, searchCode);
            var allRestaurants = _restClientWrapper.GetMultipleItemsRequest<RootObject[]>(fullApiUrl);
            return searchCode;
        }
    }

    
}