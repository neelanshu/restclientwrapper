using System.Net.Http;

namespace RestaurantsNearMe.ApiInfrastructure.Client
{
    public class DefaultClientFactory : IHttpClientFactory
    {
        public HttpClient GetClient()
        {
            return new HttpClient();
        }
    }
}
