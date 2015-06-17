using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Api.Response;
using RestaurantsNearMe.ApiInfrastructure.Client;
using RestaurantsNearMe.ApiInfrastructure.Models;
using RestaurantsNearMe.ApiInfrastructure.Serialization;
using RestaurantsNearMe.ApiInfrastructure.Tests.Fakes;

namespace RestaurantsNearMe.ApiInfrastructure.Tests.Http
{
    [TestFixture]
    public class HttpApiConnectionTests
    {
        private HttpApiConnection CreateHttpConnection(
            ICustomJsonSerializer serializer = null,
            IApiConfiguration config = null,
            IApiResponseFactory responseFactory = null,
            IHttpClientFactory clientFactory = null)
        {
            var mockSerializer = new Mock<ICustomJsonSerializer>(MockBehavior.Loose);
            mockSerializer.Setup(json => json.Serialize(It.IsAny<object>())).Returns("{}");

            return new HttpApiConnection(
                responseFactory ?? new FakeApiResponseFactory(),
                config ?? new FakeApiConfiguration(),
                serializer ?? mockSerializer.Object,
                clientFactory??new FakeHttpClientFactory());
        }

        [Test]
        public async Task ShouldSendAnHttpRequestMessageWithRequestUriSetToTheGivenUri()
        {
            var expectedUri = new Uri("http://myhost.co.uk");
            var mockHttp = new Mock<HttpClient>(MockBehavior.Loose);

            mockHttp.Setup(h => h.SendAsync(It.Is<HttpRequestMessage>(r => r.RequestUri == expectedUri),
                                             CancellationToken.None))
                    .Returns(Task.FromResult(new HttpResponseMessage()))
                    .Verifiable();

            var conn = CreateHttpConnection(clientFactory: new FakeHttpClientFactory(mockHttp.Object));
            await conn.SendRequestAsync<object>(new DefaultApiRequest(), expectedUri, HttpMethod.Get, null);

            mockHttp.Verify();
        }

        [Test]
        public async Task ShouldSendAnHttpRequestMessageWithTheGivenHttpMethod()
        {
            var expectedMethod = HttpMethod.Get;

            var mockHttp = new Mock<HttpClient>(MockBehavior.Loose);
            mockHttp.Setup(h => h.SendAsync(It.Is<HttpRequestMessage>(r => r.Method == expectedMethod),
                                            It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(new HttpResponseMessage()))
                    .Verifiable();
            var conn = CreateHttpConnection(clientFactory: new FakeHttpClientFactory(mockHttp.Object));
            await conn.SendRequestAsync<object>(new DefaultApiRequest(), new Uri("http://myhost.com"), expectedMethod, null);

            mockHttp.Verify();
        }

        [Test]
        public async Task ShouldSendAnHttpRequestMessageWithTheGivenHttpHeaders()
        {
            var expectedHeaderKey = "HeaderKey";
            var expectedHeaderValues = new[] { "Foo", "Bar" };
            IEnumerable<string> actualHeaderValues = null;
            var mockHttp = new Mock<HttpClient>(MockBehavior.Loose);
            mockHttp.Setup(h => h.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                    .Callback((HttpRequestMessage request, CancellationToken token) =>
                        actualHeaderValues = request.Headers.GetValues(expectedHeaderKey))
                    .Returns(Task.FromResult(new HttpResponseMessage()));
            var conn = CreateHttpConnection(clientFactory: new FakeHttpClientFactory(mockHttp.Object));

            await conn.SendRequestAsync<string>(
                new DefaultApiRequest(), 
                new Uri("https://api.io"),
                HttpMethod.Put,
                
                new Dictionary<string, IEnumerable<string>>
                    {
                        {expectedHeaderKey, expectedHeaderValues}
                    });

            Assert.AreEqual(expectedHeaderValues, actualHeaderValues);
        }


        [Test]
        public async Task ShouldPassTheHttpResponseMessageToTheApiResponseFactory()
        {
            var expectedResponse = new HttpResponseMessage();
            var mockHttp = new Mock<HttpClient>(MockBehavior.Loose);
            mockHttp.Setup(h => h.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(expectedResponse));

            var mockRespFact = new Mock<IApiResponseFactory>(MockBehavior.Loose);
            mockRespFact.Setup(r => r.CreateApiResponseAsync<object>(expectedResponse,It.IsAny<bool>(), It.IsAny<string>()))
                        .Returns(Task.FromResult<IApiResponse<object>>(new ApiResponse<object>()))
                        .Verifiable();

            var conn = CreateHttpConnection(responseFactory: mockRespFact.Object,clientFactory: new FakeHttpClientFactory(mockHttp.Object));

            await conn.SendRequestAsync<object>(
                new DefaultApiRequest(), 
                new Uri("https://myhost.com"),
                HttpMethod.Get,
                null);

            mockRespFact.Verify();
        }

        [Test]
        public async Task SendRequestAsync_ShouldReturnTheApiResponseReturnedByTheResponseFactory()
        {
            var expectedApiResponse = new ApiResponse<object>();
            var conn = CreateHttpConnection(responseFactory: new FakeApiResponseFactory(expectedApiResponse));

            var actualApiResponse = await conn.SendRequestAsync<object>(
                                        new DefaultApiRequest(), 
                                        new Uri("https://myhost.com"),
                                        HttpMethod.Get,
                                        null);

            Assert.AreSame(expectedApiResponse, actualApiResponse);
        } 
    }
}