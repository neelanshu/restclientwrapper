using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Api.Response;
using RestaurantsNearMe.ApiInfrastructure.Guards;
using RestaurantsNearMe.ApiInfrastructure.Helpers;
using RestaurantsNearMe.ApiInfrastructure.Models;

namespace RestaurantsNearMe.ApiInfrastructure.Client
{
    public class CustomRestClient : ICustomRestClient
    {
        private readonly IUriResolver _uriResolver;
        private readonly IHttpApiConnection _httpApiConnection;
        private readonly IApiConfiguration _apiConfiguration;

        public CustomRestClient(IHttpApiConnection httpApiConnection, IApiConfiguration apiConfiguration, IUriResolver uriResolver)
        {
            Requires.ArgumentsToBeNotNull(httpApiConnection, "httpApiConnection");
            Requires.ArgumentsToBeNotNull(apiConfiguration, "apiRequestConfiguration");
            Requires.ArgumentsToBeNotNull(uriResolver, "uriResolver");

            _apiConfiguration = apiConfiguration;
            _httpApiConnection = httpApiConnection;
            _uriResolver = uriResolver;
        }

        public IApiConfiguration ApiConfiguration { get { return _apiConfiguration; } }

        public IHttpApiConnection HttpApiConnection { get { return _httpApiConnection; } }

        public IApiResponseStatus ApiResponseStatus { get; set; }

        public Task<T> GetAsync<T>(IApiRequest request)
        {
           ValidateRequiredFields(request);
           ShouldBe.Same(request.Method, HttpMethod.Get, "request.Method");

            return SendRequestAndGetBodyAsync<T>(request);
        }

        private async Task<T> SendRequestAndGetBodyAsync<T>(IApiRequest request)
        {
            var response = await SendRequestAsync<T>(request);
            ApiResponseStatus = new DefaultApiResponseStatus()
            {
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode
            };

            return response.BodyAsObject;
        }

        private async Task<IApiResponse<T>> SendRequestAsync<T>(IApiRequest request)
        {
            ValidateRequiredFields(request);

            SetDefaultMediaTypeForAcceptRequestHeader(request.Headers);

            request.ResourceUri = _uriResolver.ResolveUri(request.ResourceUri, request.Parameters);

            return await _httpApiConnection.SendRequestAsync<T>(request, request.ResourceUri, request.Method, request.Headers).ConfigureAwait(ApiConfiguration.CaptureSynchronizationContext);
        }

        private void  SetDefaultMediaTypeForAcceptRequestHeader(Dictionary<string, IEnumerable<string>> headers)
        {
            headers = headers ?? new Dictionary<string, IEnumerable<string>>();

            if (!headers.ContainsKey("Accept"))
            {
                headers.Add("Accept", new[] { ApiConfiguration.DefaultMediaTypeForAcceptRequestHeader });
            }
        }

        private void ValidateRequiredFields(IApiRequest request)
        {
            Requires.ArgumentsToBeNotNull(request, "request");
            if (request.PartialDeserializeResponse)
            {
                Requires.ArgumentsToBeNotNull(request.ExpectedTokenToPartialDeserialize, "request.ExpectedToken");
            }

            Requires.ArgumentsToBeNotNull(request.Method, "request.Method");
            Requires.ArgumentsToBeNotNull(request.ResourceUri, "request.RequestUri");
        }
    }
}