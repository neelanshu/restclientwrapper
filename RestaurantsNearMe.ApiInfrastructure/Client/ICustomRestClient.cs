using System;
using System.Net;
using System.Threading.Tasks;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Models;

namespace RestaurantsNearMe.ApiInfrastructure.Client
{
    /// <summary>
    /// Wrapper to expose capability to consume external APIs
    /// </summary>
    public interface ICustomRestClient
    {
        Task<T> GetAsync<T>(IApiRequest request);

        IApiConfiguration ApiConfiguration { get; }

        IHttpApiConnection HttpApiConnection { get; }

        IApiResponseStatus ApiResponseStatus { get; set; }
        //This could be extended implement PostAsync atc
    }

    public interface IApiResponseStatus
    {
        HttpStatusCode StatusCode{ get; set; }
        string ReasonPhrase { get; set; }
    }

    public class DefaultApiResponseStatus : IApiResponseStatus
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
    }
}
