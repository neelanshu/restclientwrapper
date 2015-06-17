﻿using System.Net.Http;
using System.Threading.Tasks;
using RestaurantsNearMe.ApiInfrastructure.Api.Response;
using RestaurantsNearMe.ApiInfrastructure.Models;

namespace RestaurantsNearMe.ApiInfrastructure.Tests.Fakes
{
    public class FakeApiResponseFactory : IApiResponseFactory
    {
        private readonly object _resp;

        public FakeApiResponseFactory(object resp = null)
        {
            _resp = resp;
        }

        public Task<IApiResponse<T>> CreateApiResponseAsync<T>(HttpResponseMessage response, bool partialDeserialize, string token)
        {
            var apiResponse = _resp as IApiResponse<T>;
            return Task.FromResult(apiResponse ?? new ApiResponse<T>());
        }
    }
}
