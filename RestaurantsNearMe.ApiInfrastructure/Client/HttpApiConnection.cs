using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Api.Response;
using RestaurantsNearMe.ApiInfrastructure.Guards;
using RestaurantsNearMe.ApiInfrastructure.Serialization;

namespace RestaurantsNearMe.ApiInfrastructure.Client
{
    public class HttpApiConnection : IHttpApiConnection
    {
       private readonly HttpClient _httpClient;
       private readonly IApiConfiguration _apiRequestConfiguration;
        private readonly IHttpClientFactory _factory;
        private readonly IApiResponseFactory _responseFactory;
        
        public HttpApiConnection( IApiResponseFactory responseFactory, IApiConfiguration apiRequestConfiguration, ICustomJsonSerializer jsonSerializer, IHttpClientFactory factory)
        {
            Requires.ArgumentsToBeNotNull(apiRequestConfiguration, "apiConfiguration");
            Requires.ArgumentsToBeNotNull(jsonSerializer, "jsonSerializer");
            Requires.ArgumentsToBeNotNull(responseFactory, "responseFactory");

            _apiRequestConfiguration = apiRequestConfiguration;
            _factory = factory;
            _responseFactory = responseFactory;

            _httpClient = factory.GetClient();
        }

        public async Task<IApiResponse<T>> SendRequestAsync<T>(IApiRequest apiRequest)
        {
            ValidateRequiredFields(apiRequest);

            using (var request = new HttpRequestMessage {RequestUri = apiRequest.ResourceUri, Method = apiRequest.Method})
            {
               SetRequestHeaders(apiRequest.Headers, request);

               var responseMessage = await
                    _httpClient.SendAsync(request, CancellationToken.None)
                        .ConfigureAwait(_apiRequestConfiguration.CaptureSynchronizationContext);

                return 
                    await 
                    _responseFactory.CreateApiResponseAsync<T>(responseMessage, 
                                        apiRequest.PartialDeserializeResponse,
                                        apiRequest.ExpectedTokenToPartialDeserialize)
                    .ConfigureAwait(_apiRequestConfiguration.CaptureSynchronizationContext);
            }
        }

        private void ValidateRequiredFields(IApiRequest request)
        {
            Requires.ArgumentsToBeNotNull(request, "request");
            if (request.PartialDeserializeResponse)
            {
                Requires.ArgumentsToBeNotNull(request.ExpectedTokenToPartialDeserialize, "request.ExpectedToken");
            }

            Requires.ArgumentsToBeNotNull(request.ResourceUri, "request.ResourceUri");
            Requires.ArgumentsToBeNotNull(request.Method, "request.Method");
        }

        private void  SetRequestHeaders(IDictionary<string, IEnumerable<string>> headers, HttpRequestMessage request)
        {
            headers = headers ?? new Dictionary<string, IEnumerable<string>>();

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
    }
}