using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Api.Response;
using RestaurantsNearMe.ApiInfrastructure.Client;
using RestaurantsNearMe.ApiInfrastructure.Helpers;
using RestaurantsNearMe.ApiInfrastructure.Models;
using RestaurantsNearMe.ApiInfrastructure.Tests.Fakes;

namespace RestaurantsNearMe.ApiInfrastructure.Tests.Client
{
    [TestFixture]
    public class CustomRestClientTests
    {
        private CustomRestClient CreateClient(IHttpApiConnection conn = null, IApiConfiguration config = null, IUriResolver resolver = null)
        {
            return new CustomRestClient(
                conn ?? new FakeHttpApiConnection(),
                config ?? new FakeApiConfiguration() {CaptureSynchronizationContext = false},
                resolver ?? new Mock<IUriResolver>().Object);
        }


        [Test]
        public void HttpApiConnectionPropertyShouldReturnTheSetConnection()
        {
            var expectedConnection = new FakeHttpApiConnection();
            var client = CreateClient(conn: expectedConnection);

            var actualConnection = client.HttpApiConnection;

            Assert.AreSame(expectedConnection, actualConnection);

        }

        [Test]
        public void ApiConfigurationPropertyShouldReturnTheSetConfiguration()
        {
            var expectedConfiguration = new FakeApiConfiguration();

            var client = CreateClient(config: expectedConfiguration);

            var actualConfiguration = client.ApiConfiguration;

            Assert.AreSame(expectedConfiguration, actualConfiguration);
            
        }

        [Test]
        public async Task GetAsyncMethodShouldPassIncomingUriToUriResolver()
        {
            await VerifyIncomingUrlIsPassedToUriResolver((client, apirequest) 
                => client.GetAsync<object>(apirequest));
        }

        private async Task VerifyIncomingUrlIsPassedToUriResolver(Func<CustomRestClient, IApiRequest, Task> clientAction)
        {
            var expectedUri = new Uri("http://myhost.com");
            var apiRequest = new DefaultApiRequest() {Method = HttpMethod.Get, ResourceUri = expectedUri};

            var uriResolver = new Mock<IUriResolver>();

            uriResolver.Setup(r => r.ResolveUri(apiRequest.ResourceUri, It.IsAny<IDictionary<string, string>>()))
                        .Returns(new Uri("http://myhost.com/path"))
                        .Verifiable();

            var client = CreateClient(resolver: uriResolver.Object);

            await clientAction(client , apiRequest);

            uriResolver.Verify();
        }



        [Test]
        public async Task GetAsyncMethodShouldPassGivenParametersToUriResolver()
        {
            await VerifyGivenParametersArePassedToUriResolver((client, apirequest) => client.GetAsync<object>(apirequest));
        }

        private async Task VerifyGivenParametersArePassedToUriResolver(
           Func<CustomRestClient, IApiRequest, Task> clientAction)
        {
            var expectedParams = new Dictionary<string, string>(){{"a","1"}};
            var apiRequest = new DefaultApiRequest() { Method = HttpMethod.Get, ResourceUri = new Uri("http://myhost.com"), Parameters = expectedParams};

            var uriResolver = new Mock<IUriResolver>(MockBehavior.Loose);
            uriResolver.Setup(r => r.ResolveUri(It.IsAny<Uri>(), apiRequest.Parameters))
                        .Returns(new Uri("http://host.com/path"))
                        .Verifiable();

            var client = CreateClient(resolver: uriResolver.Object);

            await clientAction(client,apiRequest);

            uriResolver.Verify();
        }

        [Test]
        public async Task GetAsyncShouldSendRequestToUriReturnedByUriResolver()
        {
            await VerifyUriReturnedByUriResolverIsPassedToHttpConnection<object>(
                (client) => client.GetAsync<object>(new DefaultApiRequest()
                {
                    Method = HttpMethod.Get,
                    ResourceUri = new Uri("http://host.com/path/")
                }));
        }

        private async Task VerifyUriReturnedByUriResolverIsPassedToHttpConnection<T>(
          Func<CustomRestClient, Task> clientAction)
        {
            var expectedUri = new Uri("http://host.com/path/?a=1");
            var uriResolver = new Mock<IUriResolver>(MockBehavior.Loose);

            var mockConnection = new Mock<IHttpApiConnection>(MockBehavior.Loose);

            uriResolver.Setup(r => r.ResolveUri(It.IsAny<Uri>(), It.IsAny<IDictionary<string, string>>()))
                        .Returns(expectedUri);

            mockConnection.Setup(c => c.SendRequestAsync<T>(It.IsAny<DefaultApiRequest>(), new Uri("http://host.com/path/?a=1")
                                        ,It.IsAny<HttpMethod>(),It.IsAny<IDictionary<string, IEnumerable<string>>>()))
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T>()))
                         .Verifiable();

            var client = CreateClient(resolver: uriResolver.Object, conn: mockConnection.Object);

            await clientAction(client);

            mockConnection.Verify();
        }

        [Test]
        public async Task GetAsyncShouldSendRequestWithHttpGetMethod()
        {
            await VerifyHttpMethodIsPassedToHttpConnection<object>(
                HttpMethod.Get,
                (client) => client.GetAsync<object>(new DefaultApiRequest()
                {
                    Method = HttpMethod.Get,
                    ResourceUri = new Uri("http://host.com/path/")
                }));
        }

        private async Task VerifyHttpMethodIsPassedToHttpConnection<T>(HttpMethod expectedHttpMethod, Func<CustomRestClient, Task> clientAction)
        {
            var mockConnection = new Mock<IHttpApiConnection>(MockBehavior.Loose);

            mockConnection.Setup(c => c.SendRequestAsync<T>(It.IsAny<DefaultApiRequest>(), It.IsAny<Uri>(), expectedHttpMethod,It.IsAny<IDictionary<string, IEnumerable<string>>>()))
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T>()))
                         .Verifiable();

            var client = CreateClient(mockConnection.Object);

            await clientAction(client);

            mockConnection.Verify();
        }


        [Test]
        public async void GetAsyncShouldReturnBodyOfResponseReturnedByHttpConnection()
        {
            await VerifyBodyOfApiResponseIsReturned(
                client => client.GetAsync<object>(new DefaultApiRequest() { Method = HttpMethod.Get, ResourceUri = new Uri("http://myhost.com") }));
        }
        
        private async Task VerifyBodyOfApiResponseIsReturned<T>(
           Func<CustomRestClient, Task<T>> clientAction)
           where T : new()
        {
            var expectedResponse = new T();
            var mockConnection = new Mock<IHttpApiConnection>(MockBehavior.Loose);
            mockConnection.Setup(c => c.SendRequestAsync<T>(
                                        It.IsAny<DefaultApiRequest>(),
                                        It.IsAny<Uri>(),
                                        It.IsAny<HttpMethod>(),
                                        It.IsAny<IDictionary<string, IEnumerable<string>>>()))
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T> { BodyAsObject = expectedResponse }));
            var client = CreateClient(mockConnection.Object);

            var actualResponse = await clientAction(client);

            Assert.AreSame(expectedResponse, actualResponse);
        }

        //[Test]
        //public async void GetAsyncShouldShouldSendRequestWithGivenHeaderSetToJson()
        //{
        //    await VerifyGivenHeaderIsPassedToHttpConnection<object>(
        //        (client, headers) => client.GetAsync<Resource>(new DefaultApiRequest() { Method = HttpMethod.Get, ResourceUri = new Uri("http://myhost.com") }));
        //}

        //private async Task VerifyGivenHeaderIsPassedToHttpConnection<T>(
        //   Func<CustomRestClient, IDictionary<string, IEnumerable<string>>, Task> clientAction)
        //{
        //    var expectedHeaderKey = "Custom Header";
        //    var expectedHeaderValue = new[] { "Custom Value 1", "Custom Value 2" };
        //    IDictionary<string, IEnumerable<string>> actualHeaders = null;
        //    var mockConnection = new Mock<IHttpApiConnection>(MockBehavior.Loose);
        //    mockConnection.Setup(c => c.SendRequestAsync<T>(
        //                                It.IsAny<DefaultApiRequest>(),
        //                                It.IsAny<Uri>(),
        //                                It.IsAny<HttpMethod>(),
        //                                It.IsAny<IDictionary<string, IEnumerable<string>>>()))
        //                 .Callback((IApiRequest request, Uri uri, HttpMethod method,IDictionary<string, IEnumerable<string>> headers) =>
        //                    actualHeaders = headers)
        //                 .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T>()));
        //    var client = CreateClient(conn: mockConnection.Object);

        //    await clientAction(
        //        client,
        //        new Dictionary<string, IEnumerable<string>>
        //            {
        //                {expectedHeaderKey, expectedHeaderValue}
        //            });

        //    Assert.AreSame(expectedHeaderValue, actualHeaders[expectedHeaderKey]);
        //}

        [Test]
        public async Task GetAsyncSetsDefaultAcceptRequestHeaderIfAcceptHeaderNotPresent()
        {
            await VerifyDefaultAcceptRequestHeaderIsSetFromApiConfiguration<object>(
                (client) => client.GetAsync<object>(new DefaultApiRequest()
                {
                    Method = HttpMethod.Get,
                    ResourceUri = new Uri("http://host.com/path/")
                }));
        }

        private async Task VerifyDefaultAcceptRequestHeaderIsSetFromApiConfiguration<T>(Func<CustomRestClient, Task> clientAction)
        {
            var expectedAcceptHeader = "application/json";
            var mockConnection = new Mock<IHttpApiConnection>(MockBehavior.Loose);

            mockConnection.Setup(
                          c => c.SendRequestAsync<T>(It.IsAny<DefaultApiRequest>(), It.IsAny<Uri>(), It.IsAny<HttpMethod>(), It.IsAny<IDictionary<string, IEnumerable<string>>>()))
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T>()))
                         .Verifiable();

            var mockApiConfiguration = new Mock<IApiConfiguration>();
            mockApiConfiguration.Setup(x => x.DefaultMediaTypeForAcceptRequestHeader).Returns(expectedAcceptHeader).Verifiable();

            var client = CreateClient(mockConnection.Object, mockApiConfiguration.Object);

            await clientAction(client);

            mockApiConfiguration.Verify();
        }
    }
}