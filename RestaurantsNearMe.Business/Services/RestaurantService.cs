using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestaurantsNearMe.ApiInfrastructure;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Client;
using RestaurantsNearMe.ApiInfrastructure.Models;
using RestaurantsNearMe.Business.Models;

namespace RestaurantsNearMe.Business.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly ICustomRestClient _customRestClient;
        private readonly IApiConfiguration _apiRequestConfiguration;
        private readonly IApplicationSettings _applicationSettings;
        
        public RestaurantService(ICustomRestClient customRestClient, IApiConfiguration apiRequestConfiguration , IApplicationSettings applicationSettings)
        {
            _customRestClient = customRestClient;
            _apiRequestConfiguration = apiRequestConfiguration;
            _applicationSettings = applicationSettings;
        }

        public async Task<GetAllRestaurantResponse> GetAllRestaurantsAsync(string searchCode)
        {
            searchCode = Regex.Replace(searchCode, @"\s+", "");
            var requestParams = CreateRequestParamsForGetAllRestaurantsAsync(searchCode);

            var restaurants =
                await
                    _customRestClient.GetAsync<IEnumerable<Restaurant>>(requestParams).ConfigureAwait(_apiRequestConfiguration.CaptureSynchronizationContext);

            var apiResponse = new GetAllRestaurantResponse()
            {
                AllRestaurants = null,
                IsError = true,
                ErrorResponse = string.Empty
            };

            if (_customRestClient.ApiResponseStatus.StatusCode == HttpStatusCode.OK)
            {
                apiResponse.AllRestaurants = restaurants.Where(x => x.AreCurrentlyAvailable);
                apiResponse.IsError = false;
            }
            else
            {
                apiResponse.ErrorResponse = _customRestClient.ApiResponseStatus.ReasonPhrase;    
            }
            
            return apiResponse;
        }

        private IApiRequest CreateRequestParamsForGetAllRestaurantsAsync(string searchCode)
        {
            var apiRequest = new DefaultApiRequest
            {
                ResourceUri = new Uri("http://api-interview.just-eat.com/restaurants"),
                Parameters = new Dictionary<string, string> {{"q", searchCode}},
                Headers =
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {"Accept", new[] {"application/json"}},
                        {"Accept-Tenant", new[] {"uk"}},
                        {"Accept-Language", new[] {"en-gb"}},
                        {"Authorization", new[] {"Basic VGVjaFRlc3RBUEk6dXNlcjI="}}
                    },

                PartialDeserializeResponse = true,
                ExpectedTokenToPartialDeserialize =  "Restaurants",
                Method = HttpMethod.Get
            };

            return apiRequest;
        }
    }
}