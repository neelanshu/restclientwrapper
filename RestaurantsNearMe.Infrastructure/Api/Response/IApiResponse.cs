using System.Collections.Generic;
using System.Net;

namespace RestaurantsNearMe.ApiInfrastructure.Api.Response
{
    public interface IApiResponse
    {
        IDictionary<string, string> Headers { get; }
        HttpStatusCode StatusCode { get; }
        string ReasonPhrase { get; set; }
        string Body { get; }
    }

    public interface IApiResponse<T> : IApiResponse
    {
        T BodyAsObject { get; }
    }
}