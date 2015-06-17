using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Api.Response;
using RestaurantsNearMe.ApiInfrastructure.Client;
using RestaurantsNearMe.ApiInfrastructure.Models;

namespace RestaurantsNearMe.ApiInfrastructure.Tests.Fakes
{
    public class FakeHttpApiConnection : IHttpApiConnection
    {
        private readonly IApiResponse _response;

        public FakeHttpApiConnection(IApiResponse response = null)
        {
            _response = response;
        }

        public Task<IApiResponse<T>> SendRequestAsync<T>(IApiRequest request,Uri resourceUri , HttpMethod httpMethod,IDictionary<string, IEnumerable<string>> headers)
        {
            var response = _response as IApiResponse<T>;
            return Task.FromResult(response ?? new ApiResponse<T>());
        }
    }
}