using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using RestaurantsNearMe.Business.Models;
using RestaurantsNearMe.Business.Services;
using RestaurantsNearMe.MVC.Web.Controllers;
using RestaurantsNearMe.MVC.Web.Models;

namespace RestaurantsNearMe.MVC.Web.Tests.Controller
{
    [TestFixture]
    public class RestaurantControllerTests
    {
     
        [Test]
        public async Task GetAllAsyncShouldCallServiceWithOutcodeSuppliedFromWeb()
        {
            var expectedOutCode = string.Empty;
            var service = new Mock<IRestaurantService>(MockBehavior.Loose);
            service.Setup(r => r.GetAllRestaurantsAsync(It.IsAny<string>()))
                                                                  .Callback((string outcode) =>  expectedOutCode = outcode)
                                                                  .Returns(Task.FromResult(new GetAllRestaurantResponse())).Verifiable();
            
            var _controller = new RestaurantController(service.Object);

            var outcodeForRe = "se12";
            await _controller.GetAllAsync(outcodeForRe);

            Assert.AreEqual(expectedOutCode,outcodeForRe);
        }


        [Test]
        public async Task GetAllAsyncShouldReturnErrorAsTrueWhenServicReturnsError()
        {
            var outcode = "se12";
            var service = new Mock<IRestaurantService>(MockBehavior.Loose);
            service.Setup(r => r.GetAllRestaurantsAsync(It.IsAny<string>()))
                        .Returns(Task.FromResult(new GetAllRestaurantResponse(){IsError = true})).Verifiable();

            var _controller = new RestaurantController(service.Object);
            var result = await _controller.GetAllAsync(outcode);

            Assert.IsNotNull(result);

            var model = (RestaurantsViewModel) ((ViewResult) result).Model;
            Assert.IsTrue(model.HasError);
        }

        [Test]
        public async Task GetAllAsyncShouldReturnErrorAsTrueWhenServicReturnsNullForRestaurants()
        {
            var outcode = "se12";
            var service = new Mock<IRestaurantService>(MockBehavior.Loose);
            service.Setup(r => r.GetAllRestaurantsAsync(It.IsAny<string>()))
                                 .Returns(Task.FromResult(new GetAllRestaurantResponse() { IsError = false,AllRestaurants = null })).Verifiable();

            var controller = new RestaurantController(service.Object);
            var result = await controller.GetAllAsync(outcode);

            Assert.IsNotNull(result);

            var model = (RestaurantsViewModel)((ViewResult)result).Model;
            Assert.IsTrue(model.HasError);
        }

        [Test]
        public async Task GetAllAsyncShouldReturnErrorAsTrueWhenServicReturnsNoRestaurants()
        {
            var outcode = "se12";
            var service = new Mock<IRestaurantService>(MockBehavior.Loose);
            service.Setup(r => r.GetAllRestaurantsAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(new GetAllRestaurantResponse() { IsError = false, AllRestaurants = new List<Restaurant>() })).Verifiable();

            var controller = new RestaurantController(service.Object);
            var result = await controller.GetAllAsync(outcode);

            Assert.IsNotNull(result);

            var model = (RestaurantsViewModel)((ViewResult)result).Model;
            Assert.IsTrue(model.HasError);
        }

        [Test]
        public async Task GetAllAsyncShouldReturnErrorMessageWhenServicReturnsErroroneousResponse()
        {
            var outcode = "se12";
            var errorFromService = "Bad Request";
            var expectedError = "Sorry, no results found for this out code. Bad Request Please search again";

            var service = new Mock<IRestaurantService>(MockBehavior.Loose);
            service.Setup(r => r.GetAllRestaurantsAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(new GetAllRestaurantResponse() { IsError = true,ErrorResponse = errorFromService})).Verifiable();

            var controller = new RestaurantController(service.Object);
            var result = await controller.GetAllAsync(outcode);

            Assert.IsNotNull(result);
            var model = (RestaurantsViewModel)((ViewResult)result).Model;

            Assert.AreEqual(expectedError, model.Error.ErrorMessage);
        }

        [Test]
        public async Task GetAllAsyncShouldReturnViewModelWithAvaialbleCuisinesAsCommaSeparatedString()
        {
            var outcode = "se12";

            var fakeRestaurants = new List<Restaurant>();
            fakeRestaurants.Add(new Restaurant(){Id = 1, Name = "1"});
            fakeRestaurants[0].CuisineTypes = new[] { new CuisineType() { Name = "Chinese" }, new CuisineType() { Name = "Spanish" }, new CuisineType() { Name = "Mexican" } };

            var expectedCuisine = "Chinese,Spanish,Mexican";

            var service = new Mock<IRestaurantService>(MockBehavior.Loose);
            service.Setup(r => r.GetAllRestaurantsAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(new GetAllRestaurantResponse() { IsError = false, AllRestaurants = fakeRestaurants})).Verifiable();

            var controller = new RestaurantController(service.Object);
            var result = await controller.GetAllAsync(outcode);

            Assert.IsNotNull(result);
            var model = (RestaurantsViewModel)((ViewResult)result).Model;

            Assert.AreEqual(expectedCuisine, model.AllRestaurant[0].AvailableCuisineTypes);
        }

        [Test]
        public async Task GetAllAsyncMapsRestaurantPropertiesCorrectly()
        {
            var outcode = "se12";

            var fakeRestaurants = new List<Restaurant>();
            fakeRestaurants.Add(new Restaurant() { Id = 1, Name = "Rest1",NumberOfRatings = 10});
            fakeRestaurants[0].CuisineTypes = new[] { new CuisineType() { Name = "Chinese" }, new CuisineType() { Name = "Spanish" }, new CuisineType() { Name = "Mexican" } };

            var service = new Mock<IRestaurantService>(MockBehavior.Loose);
            service.Setup(r => r.GetAllRestaurantsAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(new GetAllRestaurantResponse() { IsError = false, AllRestaurants = fakeRestaurants })).Verifiable();

            var controller = new RestaurantController(service.Object);
            var result = await controller.GetAllAsync(outcode);

            Assert.IsNotNull(result);
            var model = (RestaurantsViewModel)((ViewResult)result).Model;

            Assert.AreEqual(1, model.AllRestaurant[0].Id);
            Assert.AreEqual("Rest1", model.AllRestaurant[0].Name);
            Assert.AreEqual(10, model.AllRestaurant[0].Rating);
        }

        [Test]
        public async Task GetAllAsyncShouldReturnResultsOrderedByMaxRating()
        {
            var outcode = "se12";

            var fakeRestaurants = new List<Restaurant>();
            fakeRestaurants.Add(new Restaurant() { Id = 1, Name = "Rest1", NumberOfRatings = 10 });
            fakeRestaurants[0].CuisineTypes = new[] { new CuisineType() { Name = "Chinese" }, new CuisineType() { Name = "Spanish" }, new CuisineType() { Name = "Mexican" } };

            fakeRestaurants.Add(new Restaurant() { Id = 1, Name = "Rest2", NumberOfRatings = 20 });
            fakeRestaurants[1].CuisineTypes = new[] { new CuisineType() { Name = "Chinese" }, new CuisineType() { Name = "Spanish" }, new CuisineType() { Name = "Mexican" } };
            
            fakeRestaurants.Add(new Restaurant() { Id = 1, Name = "Rest3", NumberOfRatings = 15 });
           fakeRestaurants[2].CuisineTypes = new[] { new CuisineType() { Name = "Chinese" }, new CuisineType() { Name = "Spanish" }, new CuisineType() { Name = "Mexican" } };


            fakeRestaurants.Add(new Restaurant() { Id = 1, Name = "Rest4", NumberOfRatings = 5 });
            fakeRestaurants[3].CuisineTypes = new[] { new CuisineType() { Name = "Chinese" }, new CuisineType() { Name = "Spanish" }, new CuisineType() { Name = "Mexican" } };

            var service = new Mock<IRestaurantService>(MockBehavior.Loose);
            service.Setup(r => r.GetAllRestaurantsAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(new GetAllRestaurantResponse() { IsError = false, AllRestaurants = fakeRestaurants })).Verifiable();

            var controller = new RestaurantController(service.Object);
            var result = await controller.GetAllAsync(outcode);

            Assert.IsNotNull(result);
            var model = (RestaurantsViewModel)((ViewResult)result).Model;

            Assert.AreEqual(20, model.AllRestaurant[0].Rating);
            Assert.AreEqual(15, model.AllRestaurant[1].Rating);
            Assert.AreEqual(10, model.AllRestaurant[2].Rating);
            Assert.AreEqual(5, model.AllRestaurant[3].Rating);
        }
    }
}