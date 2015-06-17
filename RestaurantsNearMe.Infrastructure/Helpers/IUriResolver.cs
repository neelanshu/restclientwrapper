using System;
using System.Collections.Generic;

namespace RestaurantsNearMe.ApiInfrastructure.Helpers
{
    public interface IUriResolver
    {
        Uri ResolveUri(Uri resourceUri, IDictionary<string, string> parameters);    
    }
}