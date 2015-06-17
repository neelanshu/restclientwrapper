using RestaurantsNearMe.ApiInfrastructure.Api.Request;

namespace RestaurantsNearMe.ApiInfrastructure.Tests.Fakes
{
    public class FakeApiConfiguration : IApiConfiguration
    {
        public FakeApiConfiguration()
        {
            DefaultMediaTypeForAcceptRequestHeader = "application/json";
            CaptureSynchronizationContext = true;
        }
        public string DefaultMediaTypeForAcceptRequestHeader { get; set; }
        public bool CaptureSynchronizationContext { get; set; }
    }
}