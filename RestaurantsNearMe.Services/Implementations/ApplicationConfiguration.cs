using RestaurantsNearMe.Services.Contracts;

namespace RestaurantsNearMe.Services.Implementations
{
    public class ApplicationConfiguration : IConfiguration
    {
        public string RestaurantsResource { get { return "http://abc.xyz/restaurants?se{0}"; } set { ; } }
    }
}