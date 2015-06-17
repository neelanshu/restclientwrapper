using System;
using System.Collections.Generic;
using System.Linq;
using RestaurantsNearMe.ApiInfrastructure.Guards;

namespace RestaurantsNearMe.ApiInfrastructure.Helpers
{
    public class UriResolver : IUriResolver
    {
        public Uri ResolveUri(Uri resourceUri, IDictionary<string, string> parameters)
        {
            Requires.ArgumentsToBeNotNull(resourceUri, "resourceUri");

            if (parameters == null || !parameters.Any())
            {
                return resourceUri;
            }

            return AppendParametersAsQueryParams(resourceUri, parameters);
        }

        private Uri AppendParametersAsQueryParams(Uri resourceUri, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var uriBuilder = new UriBuilder(resourceUri);
            var existingQueryParameters = uriBuilder.Query.Replace("?", "");

            if (!string.IsNullOrEmpty(existingQueryParameters) && !existingQueryParameters.EndsWith("&"))
            {
                existingQueryParameters += "&";
            }

            var parametersWithValues = parameters.Where(kv => !string.IsNullOrEmpty(kv.Value));
            var parametersQueryString = string.Join("&", parametersWithValues.Select(kv => kv.Key + "=" + kv.Value));
            uriBuilder.Query = existingQueryParameters + parametersQueryString;
            return uriBuilder.Uri;
        }
    }
}