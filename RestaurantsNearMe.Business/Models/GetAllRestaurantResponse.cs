using System.Collections.Generic;

namespace RestaurantsNearMe.Business.Models
{
    public class GetAllRestaurantResponse
    {
        public IEnumerable<Restaurant> AllRestaurants { get; set; }
        public bool IsError { get; set; }
        public string ErrorResponse { get; set; }
    }
}