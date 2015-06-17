using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using RestaurantsNearMe.ApiInfrastructure.IOC;
using RestaurantsNearMe.Business.Services;

namespace RestaurantsNearMe.Business.IOC
{
    public class ServicesDependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container
                .Register(
                    Component.For<IApplicationSettings>().ImplementedBy<ApplicationSettings>().LifestylePerWebRequest())
                .Register(
                    Component.For<IRestaurantService>().ImplementedBy<RestaurantService>().LifestylePerWebRequest());

            container.Install(new ApiInfrastructureDependencyInstaller());

        }
    }
}