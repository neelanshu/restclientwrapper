using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.Core.Internal;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Guards;
using RestaurantsNearMe.ApiInfrastructure.Serialization;

namespace RestaurantsNearMe.ApiInfrastructure.Api.Response
{
    public class ApiResponseFactory : IApiResponseFactory
    {
        private readonly ICustomJsonSerializer _jsonSerializer;
        private readonly IApiConfiguration _apiRequestConfiguration;

        public ApiResponseFactory(ICustomJsonSerializer jsonSerializer, IApiConfiguration apiRequestConfiguration)
        {
            Requires.ArgumentsToBeNotNull(jsonSerializer, "jsonSerializer");
            Requires.ArgumentsToBeNotNull(apiRequestConfiguration, "apiConfiguration");

            _jsonSerializer = jsonSerializer;
            _apiRequestConfiguration = apiRequestConfiguration;
        }

        public async Task<IApiResponse<T>> CreateApiResponseAsync<T>(HttpResponseMessage response ,  bool partialDeserialize, string token)
        {
            string body = null;
            object bodyAsObject = null;
            var isException = false;
            const HttpStatusCode customResponseStatus = HttpStatusCode.InternalServerError;
            var customReasonPhrase = string.Empty; 

            using (var content = response.Content)
            {
                if (content != null)
                {
                    if (typeof(T) != typeof(byte[]))
                    {
                        body = await
                                response
                                .Content
                                .ReadAsStringAsync()
                                .ConfigureAwait(_apiRequestConfiguration.CaptureSynchronizationContext);
                        
                        if (body != null  && IsJsonContent(response.Content) && response.IsSuccessStatusCode)
                        {
                            if (partialDeserialize && !token.IsNullOrEmpty())
                            {
                                try
                                {
                                    bodyAsObject = _jsonSerializer.PartialDeserialize<T>(body, token);
                                }
                                catch (Exception ex)
                                {
                                    isException = true;
                                    customReasonPhrase = "Failed to parse json from response";
                                }
                            }
                            else
                            {
                                bodyAsObject = _jsonSerializer.Deserialize<T>(body);
                            }

                        }
                    }
                    else
                    {
                        bodyAsObject = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(_apiRequestConfiguration.CaptureSynchronizationContext);
                    }
                }
            }
            
            var apiResponse = new ApiResponse<T>
            {
                StatusCode =  response.StatusCode, 
                ReasonPhrase = response.ReasonPhrase,
                Body = body,
                BodyAsObject = (T)bodyAsObject,
            };


            if (isException)
            {
                apiResponse.StatusCode = customResponseStatus;
                apiResponse.ReasonPhrase= customReasonPhrase;
            }

            SetResponseHeaders(apiResponse.Headers, response);

            return apiResponse;
        }


        private void SetResponseHeaders(IDictionary<string, string> headers, HttpResponseMessage response)
        {
            foreach (var header in response.Headers)
            {
                headers.Add(header.Key, header.Value.FirstOrDefault());
            }

            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    headers.Add(header.Key, header.Value.FirstOrDefault());
                }
            }
        }

        private bool IsJsonContent(HttpContent content)
        {
            if (content.Headers.ContentType == null)
            {
                return false;
            }

            return content.Headers.ContentType.MediaType == "application/json" ||
                content.Headers.ContentType.MediaType == "text/json" ||
                   content.Headers.ContentType.MediaType == "application/javascript"; //for jsonp
        }
    }
}