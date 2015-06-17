namespace RestaurantsNearMe.ApiInfrastructure.Api.Request
{
    public interface IApiConfiguration
    {
        string DefaultMediaTypeForAcceptRequestHeader  { get; set; }

        //This is to enable async operations to use the current synchronization context
        //set false to avoid deadlocks

        bool CaptureSynchronizationContext { get; set; }
    }
}