using System.Configuration;
using RestaurantsNearMe.Business.Constants;

namespace RestaurantsNearMe.Business.Services
{
    public class ApplicationSettings : IApplicationSettings
    {
        public string RestaurantsResource { get { return ConfigurationManager.AppSettings[ServiceConstants.RestaurantsResource]; } }
        public string RestaurantsResourceAcceptTenantHeader { get { return ConfigurationManager.AppSettings[ServiceConstants.RestaurantsResourceAcceptTenantHeader]; } }
        public string RestaurantsResourceAcceptLanguageHeader { get { return ConfigurationManager.AppSettings[ServiceConstants.RestaurantsResourceAcceptLanguageHeader]; } }
        public string RestaurantsResourceAuthenticationHeader { get { return ConfigurationManager.AppSettings[ServiceConstants.RestaurantsResourceAuthenticationHeader]; } }
        public string RestaurantsResourceResponseIdentifierToken { get
        {
            return ConfigurationManager.AppSettings[ServiceConstants.RestaurantsResourceResponseIdentifierToken];
        } }
    }
}