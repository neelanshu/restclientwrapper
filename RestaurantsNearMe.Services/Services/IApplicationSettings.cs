namespace RestaurantsNearMe.Business.Services
{
    public interface IApplicationSettings
    {
        string RestaurantsResource { get; }
        string RestaurantsResourceAcceptTenantHeader { get; }
        string RestaurantsResourceAcceptLanguageHeader { get; }
        string RestaurantsResourceAuthenticationHeader { get; }
        string RestaurantsResourceResponseIdentifierToken { get; }
    }
}