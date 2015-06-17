namespace RestaurantsNearMe.Web.Models
{
    public class RestaurantModel
    {
        public int Id { get; set; }
        public string Name  { get; set; }
        public int Rating { get; set; }
        public CuisineModel[] AvailableCuisines { get; set; }
    }
}