using System.IO;

namespace RestaurantsNearMe.ApiInfrastructure.Guards
{
    internal static class ShouldBe
    {
        public static void Same(object value1, object value2, string propertyName)
        {
            if (value1 != value2)
            {
                throw new InvalidDataException(propertyName);
            }
        }

    }
}
