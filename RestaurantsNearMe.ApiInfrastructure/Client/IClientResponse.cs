using System.Net;

namespace RestaurantsNearMe.ApiInfrastructure.Client
{
    public interface IClientResponse
    {
        HttpStatusCode StatusCode{ get; set; }
        string ReasonPhrase { get; set; }
    }
}