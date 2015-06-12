using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using RestaurantsNearMe.Services.Contracts;
using RestaurantsNearMe.Services.Models;
using ServiceStack.Text;

namespace RestaurantsNearMe.Services.Implementations
{
    public class CustomRestClientWrapper : IRestClientWrapper
    {
        private readonly IClientConfiguration _clientConfiguration;

        public CustomRestClientWrapper(IClientConfiguration clientConfiguration)
        {
            _clientConfiguration = clientConfiguration ?? new DefaultClientConfiguration();
        }

        public async Task<T> GetSingleItemRequest<T>(string apiUrl) where T : class 
        {
            T result = default(T);

            using (var client = GetClientWithSetup(apiUrl))
            {
                var response = await client.GetAsync(apiUrl).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                await response.Content.ReadAsStringAsync().ContinueWith((task) =>
                {
                    if (task.IsFaulted) throw task.Exception; // add new custom exception here 

                    result = JsonSerializer.DeserializeFromString<T>(task.Result);
                });
            }
            return result;
        }

        public async Task<T[]> GetMultipleItemsRequest<T>(string apiUrl) where T : class 
        {
            T[] result = default(T[]);

            using (var client = GetClientWithSetup(apiUrl))
            {
                var response = await client.GetAsync(apiUrl).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                await response.Content.ReadAsStringAsync().ContinueWith((task) =>
                {
                    if (task.IsFaulted) throw task.Exception; // add new custom exception here 

                    result = JsonSerializer.DeserializeFromString<T[]>(task.Result);
                });
            }
            return result;
        }

        private HttpClient GetClientWithSetup(string apiUrl)
        {
            var client = new HttpClient {BaseAddress = new Uri(apiUrl)};
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(_clientConfiguration.AcceptLanguage));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_clientConfiguration.Accept)); //both to come from default client cofiguration
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_clientConfiguration.Authorization, "VGVjaFRlc3RBUEk6dXNlcjI="); //both to come from default client cofiguration
            return client;
        }
    }
}