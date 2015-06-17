namespace RestaurantsNearMe.Business.Models
{
    public class CuisineType : IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public object SeoName { get; set; }
    }
}