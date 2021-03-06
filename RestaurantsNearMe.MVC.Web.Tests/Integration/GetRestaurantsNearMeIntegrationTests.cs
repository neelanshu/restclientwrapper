﻿using System.Threading.Tasks;
using System.Web.Mvc;
using NUnit.Framework;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Api.Response;
using RestaurantsNearMe.ApiInfrastructure.Client;
using RestaurantsNearMe.ApiInfrastructure.Helpers;
using RestaurantsNearMe.ApiInfrastructure.Serialization;
using RestaurantsNearMe.Business.Services;
using RestaurantsNearMe.MVC.Web.Controllers;
using RestaurantsNearMe.MVC.Web.Models;

namespace RestaurantsNearMe.MVC.Web.Tests.Integration
{
    [TestFixture]
    public class GetRestaurantsNearMeIntegrationTests
    {
        private RestaurantController controller;

        [SetUp]
        public void SetUp()
        {
            var restaurantService =
               new RestaurantService(
                   new GenericRestClient(
                       new HttpApiConnection(
                           new ApiResponseFactory(new DefaultJsonSerializer(), new DefaultApiRequestConfiguration()),
                           new DefaultApiRequestConfiguration(), new DefaultJsonSerializer(),
                           new DefaultClientFactory()), new DefaultApiRequestConfiguration(), new UriResolver()),
                   new DefaultApiRequestConfiguration());

            controller = new RestaurantController(restaurantService);

        }
        [Test]
        public async Task GetAllAsyncShouldSuccessfullyGetAllRestaurantsFromApiForAnOutcode()
        {
            var result = await controller.GetAllAsync("se19");

            Assert.IsNotNull(result);

            var model = (RestaurantsViewModel)((ViewResult)result).Model;

            Assert.IsNotNull(model.AllRestaurant);
            Assert.Greater(model.AllRestaurant.Count,0);
            Assert.False(model.HasError);
        }

        [Test]
        public async Task GetAllAsyncShouldNotReturnAnyRestaurantsFromApiForAnOutcodeWhichIsEmpty()
        {

            var result = await controller.GetAllAsync(string.Empty);

            Assert.IsNotNull(result);

            var model = (RestaurantsViewModel)((ViewResult)result).Model;

            Assert.IsEmpty(model.AllRestaurant);
            Assert.True(model.HasError);
        }

        [Test]
        public async Task GetAllAsyncShouldNotReturnAnyRestaurantsFromApiForAnOutcodeWhichIsNotPostcode()
        {
            var result = await controller.GetAllAsync("lon");

            Assert.IsNotNull(result);

            var model = (RestaurantsViewModel)((ViewResult)result).Model;

            Assert.IsEmpty(model.AllRestaurant);
            Assert.True(model.HasError);
        }
    }
}