using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.GeoLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.MAUIBlazor.MultiPlatformServices
{
    public class MauiGeoLocationService : IGeoLocationService
    {
        public async Task<GeoCoordinates> GetCurrentPositionAsync()
        {
            var locationTaskCompletionSource = new TaskCompletionSource<Location>();
            App.Current!.Dispatcher.Dispatch(async () =>
            {
                var result = await Geolocation.GetLocationAsync();
                if (result != null)
                    locationTaskCompletionSource.SetResult(result);
            });
            await locationTaskCompletionSource.Task.ConfigureAwait(false);
            var geoCoorindates = new GeoCoordinates
            {
                Latitude = locationTaskCompletionSource.Task.Result.Latitude,
                Longitude = locationTaskCompletionSource.Task.Result.Longitude
            };
            return geoCoorindates;
        }
    }
}
