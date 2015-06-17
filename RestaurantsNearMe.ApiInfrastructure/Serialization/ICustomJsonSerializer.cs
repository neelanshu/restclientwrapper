namespace RestaurantsNearMe.ApiInfrastructure.Serialization
{
    public interface ICustomJsonSerializer
    {
        string Serialize(object value);
        T Deserialize<T>(string json);
        T PartialDeserialize<T>(string json, string token);
    }
}