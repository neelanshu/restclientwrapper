//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;
//using Moq;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using NUnit.Framework;
//using RestaurantsNearMe.Services.Models;
//using ServiceStack.Text;


//namespace RestaurantsNearMe.Services.Tests
//{

//    [TestFixture]
//    public class TestJsonConvertion
//    {
//        [Test]
//        public void TestHowJsonConverts()
//        {
//            using (var r = new StreamReader(@"C:\Users\nsharma\Desktop\PrivateGitRepos\RestaurantsNearMe\RestaurantsNearMe.Services.Tests\resjson.txt"))
//            {
//                string json = r.ReadToEnd();
//                JToken objJToken = JObject.Parse(json);
//                var rests = objJToken.SelectToken("Restaurants")
//                                .ToObject<Restaurant[]>()
//                                .Where(x=>x.IsOpenNow 
//                                    && 
//                                    x.IsOpenNowForCollection 
//                                    && 
//                                    x.IsOpenNowForDelivery 
//                                    && 
//                                    !x.IsTemporarilyOffline)
//                                .OrderByDescending(x=>x.NumberOfRatings);

//                foreach (var item in rests)
//                {
//                    Console.WriteLine(item.Id + ",  " + item.Name + ", " + item.NumberOfRatings +
//                                      ", Cuisines avaialable [" +
//                                      string.Join(",", item.CuisineTypes.Select(x => x.Name)) +  " ]");
//                }
//            }

//            Assert.AreEqual("a","a");
//        }

//        private HttpClient GetClientWithSetup(string apiUrl)
//        {
//            var client = new HttpClient { BaseAddress = new Uri(apiUrl) };
//            client.DefaultRequestHeaders.Clear();
//            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-gb"));
//            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //both to come from default client cofiguration
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "VGVjaFRlc3RBUEk6dXNlcjI="); //both to come from default client cofiguration
//            return client;
//        }
//    }
//}
