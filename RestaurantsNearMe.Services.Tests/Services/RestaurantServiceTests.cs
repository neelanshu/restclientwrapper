using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Client;
using RestaurantsNearMe.Business.Models;
using RestaurantsNearMe.Business.Services;

namespace RestaurantsNearMe.Business.Tests.Services
{
    [TestFixture]
    public class RestaurantServiceTests
    {
        
        private RestaurantService CreateService(IApiConfiguration config= null, ICustomRestClient client = null, IApplicationSettings appsettings = null)
        {
            return new RestaurantService(
                client?? new Mock<ICustomRestClient>().Object,
                config ?? new Mock<IApiConfiguration>().Object,
                appsettings ?? new Mock<IApplicationSettings>().Object);
        }

        [Test]
        public async Task GetAllRestaurantsAsyncShouldPassCorrectRequestToClient()
        {
            const string searchCode = "se12";
            IApiRequest actualRequest = new DefaultApiRequest(); 
            var expectedApiRequest = CreateRequestParamsForGetAllRestaurantsAsyncForJustEat(searchCode);

            //var appsettings = new Mock<IApplicationSettings>();
            //appsettings.SetupGet(x => x.RestaurantsResource).Returns("http://api-interview.just-eat.com/restaurants");
            //appsettings.SetupGet(x => x.RestaurantsResourceAcceptLanguageHeader).Returns("en-gb");
            //appsettings.SetupGet(x => x.RestaurantsResourceAcceptTenantHeader).Returns("uk");
            //appsettings.SetupGet(x => x.RestaurantsResourceResponseIdentifierToken).Returns("Restaurant");
            //appsettings.SetupGet(x => x.RestaurantsResourceAuthenticationHeader).Returns("Basic VGVjaFRlc3RBUEk6dXNlcjI=");

            var client = new Mock<ICustomRestClient>(MockBehavior.Loose);
            client.Setup(r => r.GetAsync<IEnumerable<Restaurant>>(It.IsAny<IApiRequest>()))
                                                                  .Callback((IApiRequest re) => actualRequest = re)      
                                                                  .Returns(Task.FromResult<IEnumerable<Restaurant>>(new List<Restaurant>())).Verifiable();
            client.Setup(r => r.ApiResponseStatus).Returns(new DefaultApiResponseStatus());

            var service = CreateService(client: client.Object);//, appsettings:appsettings.Object);

            await service.GetAllRestaurantsAsync(searchCode);

            Assert.AreEqual(expectedApiRequest.ResourceUri,actualRequest.ResourceUri);
            Assert.AreEqual(expectedApiRequest.Method, actualRequest.Method);
            Assert.AreEqual(expectedApiRequest.Headers, actualRequest.Headers);
            Assert.AreEqual(expectedApiRequest.Parameters, actualRequest.Parameters);
            Assert.AreEqual(expectedApiRequest.ExpectedTokenToPartialDeserialize, actualRequest.ExpectedTokenToPartialDeserialize);
            Assert.AreEqual(expectedApiRequest.PartialDeserializeResponse, actualRequest.PartialDeserializeResponse);
        }

        private IApiRequest CreateRequestParamsForGetAllRestaurantsAsyncForJustEat(string searchCode)
        {
            var apiRequest = new DefaultApiRequest
            {
                ResourceUri = new Uri("http://api-interview.just-eat.com/restaurants"),
                Parameters = new Dictionary<string, string> { { "q", searchCode } },
                Headers =
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {"Accept", new[] {"application/json"}},
                        {"Accept-Tenant", new[] {"uk"}},
                        {"Accept-Language", new[] {"en-gb"}},
                        {"Authorization", new[] {"Basic VGVjaFRlc3RBUEk6dXNlcjI="}}
                    },

                PartialDeserializeResponse = true,
                ExpectedTokenToPartialDeserialize = "Restaurant",
                Method = HttpMethod.Get
            };

            return apiRequest;
        }

        [Test]
        public async Task GetAllRestaurantsAsyncShouldCallClient()
        {
            const string searchCode = "se12";
            var client = new Mock<ICustomRestClient>(MockBehavior.Loose);
            client.Setup(r => r.GetAsync<IEnumerable<Restaurant>>(It.IsAny<IApiRequest>()))
                                                                  .Returns(Task.FromResult<IEnumerable<Restaurant>>(new List<Restaurant>())).Verifiable();
            client.Setup(r => r.ApiResponseStatus).Returns(new DefaultApiResponseStatus());

            var service = CreateService(client: client.Object);

            await service.GetAllRestaurantsAsync(searchCode);

            client.VerifyAll();
        }

        [Test]
        public async Task GetAllRestaurantsAsyncShouldRemoveWhitespacesFromSearchCodeBeforeCreatingClientRequest()
        {
            string searchCode = "se 12";
            var expectedParameters = new Dictionary<string, string>
            {
                {
                    "q", "se12"
                }
            };

            var actualParameters = new Dictionary<string, string>();

            var client = new Mock<ICustomRestClient>(MockBehavior.Loose);
            client.Setup(r => r.GetAsync<IEnumerable<Restaurant>>(It.IsAny<IApiRequest>()))
                                                                .Callback((IApiRequest re) => actualParameters = re.Parameters)
                                                                .Returns(Task.FromResult<IEnumerable<Restaurant>>(new List<Restaurant>())).Verifiable();
            client.Setup(r => r.ApiResponseStatus).Returns(new DefaultApiResponseStatus());

            var service = CreateService(client: client.Object);

            await service.GetAllRestaurantsAsync(searchCode);

            Assert.AreEqual(expectedParameters, actualParameters);
        }

        [Test]
        public async Task SuccessfulResponseFromClientCreatesResponseWithNoErrorAndRestaurantsList()
        {
            const string searchCode = "se12";
            var restaurantsFromClient = new List<Restaurant>();
            restaurantsFromClient.Add(new Restaurant(){Id = 1, Name = "2", IsOpenNow = true,IsOpenNowForCollection = true, IsOpenNowForDelivery = true, IsTemporarilyOffline = false});
            restaurantsFromClient.Add(new Restaurant() { Id = 2, Name = "2", IsOpenNow = true, IsOpenNowForCollection = true, IsOpenNowForDelivery = true, IsTemporarilyOffline = true });
            restaurantsFromClient.Add(new Restaurant() { Id = 3, Name = "3", IsOpenNow = true, IsOpenNowForCollection = true, IsOpenNowForDelivery = true, IsTemporarilyOffline = false });
            restaurantsFromClient.Add(new Restaurant() { Id = 4, Name = "4", IsOpenNow = true, IsOpenNowForCollection = true, IsOpenNowForDelivery = true, IsTemporarilyOffline = false });

            var client = new Mock<ICustomRestClient>(MockBehavior.Loose);
            client.Setup(r => r.GetAsync<IEnumerable<Restaurant>>(It.IsAny<IApiRequest>()))
                                                                  .Returns(Task.FromResult<IEnumerable<Restaurant>>(restaurantsFromClient)).Verifiable();
            client.Setup(r => r.ApiResponseStatus).Returns(new DefaultApiResponseStatus() {StatusCode = HttpStatusCode.OK});

            var service = CreateService(client: client.Object);

            var results = await service.GetAllRestaurantsAsync(searchCode);

            Assert.AreEqual(3,results.AllRestaurants.Count());
            Assert.AreEqual(false, results.IsError);
        }

        [Test]
        public async Task UnsuccessfulResponseFromClientCreatesResponseWithErrorAndNullRestaurantsList()
        {
            const string searchCode = "se12";
            var restaurantsFromClient = new List<Restaurant>();
            restaurantsFromClient.Add(new Restaurant() { Id = 1, Name = "2", IsOpenNow = true, IsOpenNowForCollection = true, IsOpenNowForDelivery = true, IsTemporarilyOffline = false });
            restaurantsFromClient.Add(new Restaurant() { Id = 2, Name = "2", IsOpenNow = true, IsOpenNowForCollection = true, IsOpenNowForDelivery = true, IsTemporarilyOffline = true });
            restaurantsFromClient.Add(new Restaurant() { Id = 3, Name = "3", IsOpenNow = true, IsOpenNowForCollection = true, IsOpenNowForDelivery = true, IsTemporarilyOffline = false });
            restaurantsFromClient.Add(new Restaurant() { Id = 4, Name = "4", IsOpenNow = true, IsOpenNowForCollection = true, IsOpenNowForDelivery = true, IsTemporarilyOffline = false });

            var client = new Mock<ICustomRestClient>(MockBehavior.Loose);
            client.Setup(r => r.GetAsync<IEnumerable<Restaurant>>(It.IsAny<IApiRequest>()))
                                                                  .Returns(Task.FromResult<IEnumerable<Restaurant>>(restaurantsFromClient)).Verifiable();
            client.Setup(r => r.ApiResponseStatus).Returns(new DefaultApiResponseStatus() { StatusCode = HttpStatusCode.BadGateway});

            var service = CreateService(client: client.Object);

            var results = await service.GetAllRestaurantsAsync(searchCode);

            Assert.IsNull(results.AllRestaurants);
            Assert.AreEqual(true, results.IsError);
        }
    }
}