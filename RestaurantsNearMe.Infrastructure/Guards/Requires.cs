using System;

namespace RestaurantsNearMe.ApiInfrastructure.Guards
{
    internal static class Requires
    {
        public static void ArgumentsToBeNotNull(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void ArgumentsToBeNotNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("String cannot be null or empty", name);
            }
        }
    }
}
