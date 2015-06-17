using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Api.Response;
using RestaurantsNearMe.ApiInfrastructure.Client;
using RestaurantsNearMe.ApiInfrastructure.Helpers;
using RestaurantsNearMe.ApiInfrastructure.Models;
using RestaurantsNearMe.ApiInfrastructure.Serialization;

namespace RestaurantsNearMe.ApiInfrastructure.IOC
{
    public class ApiInfrastructureDependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container
                .Register(
                    Component.For<IHttpClientFactory>().ImplementedBy<DefaultClientFactory>().LifestylePerWebRequest())
                .Register(
                    Component.For<IApiConfiguration>().ImplementedBy<DefaultApiRequestConfiguration>().LifestylePerWebRequest())
                .Register(
                    Component.For<IUriResolver>().ImplementedBy<UriResolver>().LifestylePerWebRequest())
                .Register(
                    Component.For<ICustomJsonSerializer>().ImplementedBy<DefaultJsonSerializer>().LifestyleSingleton())
                .Register(
                    Component.For<IApiResponseFactory>().ImplementedBy<ApiResponseFactory>().LifestylePerWebRequest())
                .Register(
                    Component.For<IHttpApiConnection>().ImplementedBy<HttpApiConnection>().LifestylePerWebRequest())
                .Register(
                    Component.For<IClient>().ImplementedBy<GenericRestClient>().LifestylePerWebRequest());
        }
    }
}