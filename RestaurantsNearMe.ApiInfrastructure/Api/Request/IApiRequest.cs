using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RestaurantsNearMe.ApiInfrastructure.Api.Request
{
    //Abstraction on custom request parameters 
    public interface IApiRequest
    {
        Uri ResourceUri { get; set; }

        Dictionary<string, string> Parameters { get; set; }

        Dictionary<string, IEnumerable<string>> Headers { get; set; }

        bool PartialDeserializeResponse { get; set; }

        string ExpectedTokenToPartialDeserialize { get; set; }

        HttpMethod Method { get; set; }

    }
}