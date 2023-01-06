namespace FairPlaySocial.Common.Interfaces.Services
{
    public interface IGeoLocationService
    {
        Task<GeoCoordinates> GetCurrentPositionAsync();
    }

    public class GeoCoordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
