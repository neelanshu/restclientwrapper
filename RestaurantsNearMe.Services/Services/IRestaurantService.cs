using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantsNearMe.Business.Models;

namespace RestaurantsNearMe.Business.Services
{
    public interface IRestaurantService
    {
        Task<GetAllRestaurantResponse> GetAllRestaurantsAsync(string searchCode);
    }
}