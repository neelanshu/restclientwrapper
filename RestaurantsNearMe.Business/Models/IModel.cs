using System.Collections.Generic;

namespace RestaurantsNearMe.Business.Models
{
    //marker interface for business models
    public interface IModel
    {
         
    }

    public class RootObject
    {
        public string ShortResultText { get; set; }
        public List<Restaurant> Restaurants { get; set; }
    }
}