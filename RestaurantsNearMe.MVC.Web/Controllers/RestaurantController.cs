using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using RestaurantsNearMe.Business.Services;
using RestaurantsNearMe.MVC.Web.Models;

namespace RestaurantsNearMe.MVC.Web.Controllers
{
    public class RestaurantController : AsyncController
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetAllAsync(string outCode)
        {
            var viewModel = new RestaurantsViewModel()
            {
                AllRestaurant = new List<RestaurantModel>(),
                Error = new ErrorModel(),
            };

            var serviceResponse = await _restaurantService.GetAllRestaurantsAsync(outCode);
            viewModel.Error.ErrorMessage = serviceResponse.ErrorResponse;
            viewModel.HasError = serviceResponse.IsError;

            if ((serviceResponse.AllRestaurants == null) ||
                (serviceResponse.AllRestaurants != null && !serviceResponse.AllRestaurants.Any()))
            {
                viewModel.HasError = true;
                viewModel.Error.ErrorMessage = "Sorry, no results found for this out code. " + viewModel.Error.ErrorMessage + " Please search again";
            }

            if (!viewModel.HasError)
                if (serviceResponse.AllRestaurants != null)
                    foreach (var rests in serviceResponse.AllRestaurants.OrderByDescending(x=>x.NumberOfRatings) )
                    {
                        viewModel.AllRestaurant.Add(new RestaurantModel(){Id=rests.Id,Name=rests.Name,Rating=rests.NumberOfRatings,
                            AvailableCuisines = rests.CuisineTypes.Select(x=> new CuisineModel {Id = x.Id, Name = x.Name}).ToArray()});
                    }

            return View("Index", viewModel);
        }
    }
}
