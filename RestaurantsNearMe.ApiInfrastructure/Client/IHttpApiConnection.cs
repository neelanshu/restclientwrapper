using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Api.Response;

namespace RestaurantsNearMe.ApiInfrastructure.Client
{
    public interface IHttpApiConnection
    {
        Task<IApiResponse<T>> SendRequestAsync<T>(IApiRequest request, Uri resourceUri, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers);
    }
}