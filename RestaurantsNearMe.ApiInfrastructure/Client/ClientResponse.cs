using System.Net;

namespace RestaurantsNearMe.ApiInfrastructure.Client
{
    public class ClientResponse : IClientResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
    }
}