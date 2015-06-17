using System.Linq;

namespace RestaurantsNearMe.MVC.Web.Models
{
    public class RestaurantModel
    {
        public int Id { get; set; }
        public string Name  { get; set; }
        public int Rating { get; set; }
        public CuisineModel[] AvailableCuisines { get; set; }
        public string AvailableCuisineTypes { get { return string.Join(",", this.AvailableCuisines.Select(x => x.Name)); } }
    }
}