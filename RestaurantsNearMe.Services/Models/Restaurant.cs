namespace RestaurantsNearMe.Business.Models
{
    public class Restaurant :IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CuisineType[] CuisineTypes { get; set; }
        public bool IsOpenNow { get; set; }
        public bool IsTemporarilyOffline { get; set; }
        public bool IsOpenNowForDelivery { get; set; }
        public bool IsOpenNowForCollection { get; set; }
        public int NumberOfRatings { get; set; }

        public bool AreCurrentlyAvailable { get
        {
            return IsOpenNow
                   &&
                   IsOpenNowForCollection
                   &&
                   IsOpenNowForDelivery
                   &&
                   !IsTemporarilyOffline;
        } }
    }
}