using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RestaurantsNearMe.ApiInfrastructure.Helpers;

namespace RestaurantsNearMe.ApiInfrastructure.Tests.Helpers
{
    [TestFixture]
    public class UriResolverTests
    {
        private IUriResolver _sut;

        [SetUp]
        public void SetUp()
        {
            _sut= new UriResolver();    
        }


        [TestCase("http://myhost.com/path", null, "http://myhost.com/path")]
        [TestCase("http://myhost.com/path",  "", "http://myhost.com/path")]
        [TestCase("http://myhost.com/path", "foo=foovalue;bar=barvalue", "http://myhost.com/path?foo=foovalue&bar=barvalue")]
        [TestCase("http://myhost.com/path?existing=true", "foo=foovalue", "http://myhost.com/path?existing=true&foo=foovalue")]
        [TestCase("http://myhost.com/path", "foo=foo value!", "http://myhost.com/path?foo=foo%20value%21")]
        [TestCase("http://myhost.com/path", "foo=foovalue;bar=", "http://myhost.com/path?foo=foovalue")]
        
        public void ResolveUriShouldResolveCorrectUriForAGivenUriAndSetOfQueryStringParameters(string href,
                string parametersText,
                string expectedUrl)
        {
            var expectedUri = new Uri(expectedUrl);
            var actualUri = _sut.ResolveUri(
                                new Uri(href), 
                                ConvertParametersToDictionary(parametersText));

            Assert.AreEqual(expectedUri, actualUri);
        }

        private IDictionary<string, string> ConvertParametersToDictionary(string parametersText)
        {
            if (parametersText == null)
            {
                return new Dictionary<string, string>();
            }

            return parametersText
                    .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                    .ToDictionary(kv => kv.Split('=')[0], kv => kv.Split('=')[1]);
        }
    }
}