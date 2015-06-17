using System.Net.Http;
using System.Threading.Tasks;

namespace RestaurantsNearMe.ApiInfrastructure.Api.Response
{
    public interface IApiResponseFactory
    {
        Task<IApiResponse<T>> CreateApiResponseAsync<T>(HttpResponseMessage response, bool partialDeserialize, string token);
    }
}