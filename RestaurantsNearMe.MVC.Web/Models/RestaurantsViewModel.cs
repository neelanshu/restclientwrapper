using System.Collections.Generic;

namespace RestaurantsNearMe.MVC.Web.Models
{
    public class RestaurantsViewModel
    {
        public IList<RestaurantModel> AllRestaurant { get; set; }
        public bool HasError
        {
            get; set; }
        public ErrorModel Error { get; set; }
    }
}