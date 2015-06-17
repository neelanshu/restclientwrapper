using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestaurantsNearMe.ApiInfrastructure.Serialization
{
    public class DefaultJsonSerializer: ICustomJsonSerializer
    {
        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public T PartialDeserialize<T>(string json, string token)
        {
            return JObject.Parse(json).SelectToken(token).ToObject<T>();
        }
    }
}