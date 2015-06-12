using System.Threading.Tasks;
using RestaurantsNearMe.Services.Models;

namespace RestaurantsNearMe.Services.Contracts
{
    public interface IRestClientWrapper
    {
        Task<T> GetSingleItemRequest<T>(string apiUrl) where T : class;
        Task<T[]> GetMultipleItemsRequest<T>(string apiUrl) where T : class ;
    }
}
