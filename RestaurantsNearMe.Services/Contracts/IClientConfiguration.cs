namespace RestaurantsNearMe.Services.Contracts
{
    public interface IClientConfiguration
    {
        string ContentType { get; set; }
        string Accept{ get; set; }
        string AcceptTenant { get; set; }  //uk
        string AcceptLanguage { get; set; }// en-GB
        string Authorization { get; set; } //Basic VGVjaFRlc3RBUEk6dXNlcjI=
        //string Host { get; set; } //api-interview.just-eat.com
    }
}