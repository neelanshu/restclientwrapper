using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantsNearMe.Services.Contracts;

namespace RestaurantsNearMe.Services.Implementations
{
    public class ClientConfiguration : IClientConfiguration
    {
        public string ContentType { get; set; }
        public string Accept { get; set; }
        public string AcceptTenant { get; set; }
        public string AcceptLanguage { get; set; }
        public string Authorization { get; set; }
    }
}
