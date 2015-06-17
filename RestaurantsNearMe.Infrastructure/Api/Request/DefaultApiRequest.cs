using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RestaurantsNearMe.ApiInfrastructure.Api.Request
{
    public class DefaultApiRequest : IApiRequest
    {
        public Uri ResourceUri { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public Dictionary<string, IEnumerable<string>> Headers { get; set; }
        public bool PartialDeserializeResponse { get; set; }
        public string ExpectedTokenToPartialDeserialize { get; set; }
        public HttpMethod Method { get; set; }
    }
}