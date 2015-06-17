namespace RestaurantsNearMe.ApiInfrastructure.Api.Request
{
    public class DefaultApiRequestConfiguration : IApiConfiguration
    {
        public string DefaultMediaTypeForAcceptRequestHeader { get { return "application/json";  } set { ; } }

        public bool CaptureSynchronizationContext { get { return false;  }set { ; } }
    }
}