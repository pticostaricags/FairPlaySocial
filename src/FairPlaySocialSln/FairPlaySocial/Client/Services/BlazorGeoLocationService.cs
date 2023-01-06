using BrowserInterop.Extensions;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Common.CustomExceptions;
using Microsoft.JSInterop;

namespace FairPlaySocial.Client.Services
{
    public class BlazorGeoLocationService : IGeoLocationService
    {
        private readonly IJSRuntime JSRuntime;
        public BlazorGeoLocationService(IJSRuntime jsRuntime)
        {
            this.JSRuntime = jsRuntime;
        }
        public async Task<GeoCoordinates> GetCurrentPositionAsync()
        {
            var window = await JSRuntime.Window();
            var navigator = await window.Navigator();
            var geoLocation = navigator.Geolocation;
            var currentPosition = await navigator.Geolocation.GetCurrentPosition();
            if (currentPosition.Error != null)
            {
                string message = "Unable to retrieve location, please make sure to give location access to the app";
                throw new CustomValidationException(message);
            }
            return new GeoCoordinates()
            {
                Latitude = currentPosition.Location.Coords.Latitude,
                Longitude = currentPosition.Location.Coords.Longitude
            };
        }
    }
}
