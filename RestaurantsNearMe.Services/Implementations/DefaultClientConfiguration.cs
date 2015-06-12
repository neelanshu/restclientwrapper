using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantsNearMe.Services.Contracts;

namespace RestaurantsNearMe.Services.Implementations
{
    public class DefaultClientConfiguration  : IClientConfiguration
    {
        public string ContentType { get; set; }
        public string Accept { get { return "application/json"; } set { ; } }
        public string AcceptTenant { get { return "en-gb"; } set { ; } }
        public string AcceptLanguage { get { return "uk"; } set { ; } }
        public string Authorization { get { return "Basic"; } set { ; } }
    }
}
