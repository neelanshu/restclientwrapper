using System.Net.Http;

namespace RestaurantsNearMe.ApiInfrastructure.Client
{
    public interface IHttpClientFactory
    {
        HttpClient GetClient();
    }
}