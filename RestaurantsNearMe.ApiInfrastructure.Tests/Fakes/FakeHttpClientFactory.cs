using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using RestaurantsNearMe.ApiInfrastructure.Client;

namespace RestaurantsNearMe.ApiInfrastructure.Tests.Fakes
{
    public class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _httpClient;

        public FakeHttpClientFactory(HttpClient http = null)
        {
            var mockClient = new Mock<HttpClient>(MockBehavior.Loose);
            mockClient.Setup(h => h.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.FromResult(new HttpResponseMessage()));
            _httpClient = http ?? mockClient.Object;
        }

        public HttpClient GetClient()
        {

            return _httpClient;
        }
    }
}