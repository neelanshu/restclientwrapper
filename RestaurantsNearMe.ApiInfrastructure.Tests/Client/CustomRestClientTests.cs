using System;
using System.Collections.Generic;
using System.Linq;
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
        private GenericRestClient CreateClient(IHttpApiConnection conn = null, IApiConfiguration config = null, IUriResolver resolver = null)
        {
            return new GenericRestClient(
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

        private async Task VerifyIncomingUrlIsPassedToUriResolver(Func<GenericRestClient, IApiRequest, Task> clientAction)
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
           Func<GenericRestClient, IApiRequest, Task> clientAction)
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
          Func<GenericRestClient, Task> clientAction)
        {
            var expectedUri = new Uri("http://host.com/path/?a=1");
            var uriToGetFromRequest = new Uri("http://m");

            var uriResolver = new Mock<IUriResolver>(MockBehavior.Loose);
            var mockConnection = new Mock<IHttpApiConnection>(MockBehavior.Loose);

            uriResolver.Setup(r => r.ResolveUri(It.IsAny<Uri>(), It.IsAny<IDictionary<string, string>>()))
                        .Returns(expectedUri);

            mockConnection.Setup(c => c.SendRequestAsync<T>(It.IsAny<IApiRequest>()))
                          .Callback((IApiRequest re )=>uriToGetFromRequest = re.ResourceUri)
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T>()))
                         .Verifiable();

            var client = CreateClient(resolver: uriResolver.Object, conn: mockConnection.Object);

            await clientAction(client);

            Assert.AreEqual(expectedUri,uriToGetFromRequest);
            //mockConnection.Verify();
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

        private async Task VerifyHttpMethodIsPassedToHttpConnection<T>(HttpMethod expectedHttpMethod, Func<GenericRestClient, Task> clientAction)
        {
            var actualMethod = HttpMethod.Trace;

            var mockConnection = new Mock<IHttpApiConnection>(MockBehavior.Loose);

            mockConnection.Setup(c => c.SendRequestAsync<T>(It.IsAny<IApiRequest>()))
                          .Callback((IApiRequest re )=>actualMethod = re.Method)
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T>()))
                         .Verifiable();

            var client = CreateClient(mockConnection.Object);

            await clientAction(client);

            Assert.AreEqual(expectedHttpMethod,actualMethod);
        }


        [Test]
        public async Task GetAsyncShouldReturnBodyOfResponseReturnedByHttpConnection()
        {
            await VerifyBodyOfApiResponseIsReturned(
                client => client.GetAsync<object>(new DefaultApiRequest() { Method = HttpMethod.Get, ResourceUri = new Uri("http://myhost.com") }));
        }
        
        private async Task VerifyBodyOfApiResponseIsReturned<T>(
           Func<GenericRestClient, Task<T>> clientAction)
           where T : new()
        {
            var expectedResponse = new T();
            var mockConnection = new Mock<IHttpApiConnection>(MockBehavior.Loose);
            mockConnection.Setup(c => c.SendRequestAsync<T>(It.IsAny<IApiRequest>()))
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T> { BodyAsObject = expectedResponse }));
            var client = CreateClient(mockConnection.Object);

            var actualResponse = await clientAction(client);

            Assert.AreSame(expectedResponse, actualResponse);
        }

        [Test]
        public async Task GetAsyncShouldSendRequestWithAcceptHeaderSetToJson()
        {
            await VerifyGivenHeaderIsPassedToHttpConnection<Resource>(
            (client) => client.GetAsync<Resource>(new DefaultApiRequest()
            {
                Method = HttpMethod.Get,
                ResourceUri = new Uri("http://host.com/path/"),
                Headers = null
            }));
        }

        private async Task VerifyGivenHeaderIsPassedToHttpConnection<T>(Func<GenericRestClient, Task> clientAction)
        {
           
            IDictionary<string, IEnumerable<string>> actualHeaders = null;
            var mockConnection = new Mock<IHttpApiConnection>(MockBehavior.Loose);

            mockConnection.Setup(c => c.SendRequestAsync<T>(It.IsAny<IApiRequest>()))
                         .Callback((IApiRequest request) => actualHeaders= request.Headers)
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T>()));

            var client = CreateClient(mockConnection.Object);

            await clientAction(client);
            
            mockConnection.Verify();
        }
    }
}