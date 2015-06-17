using System.Threading.Tasks;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;

namespace RestaurantsNearMe.ApiInfrastructure.Client
{
    /// <summary>
    /// Wrapper to expose capability to consume external APIs
    /// </summary>
    public interface IClient
    {
        Task<T> GetAsync<T>(IApiRequest request);

        IApiConfiguration ApiConfiguration { get; }

        IHttpApiConnection HttpApiConnection { get; }

        IClientResponse ApiResponseStatus { get; set; }

        //could be extended implement PostAsync atc
    }
}
