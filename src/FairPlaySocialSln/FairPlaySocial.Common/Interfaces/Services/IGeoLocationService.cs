using FairPlaySocial.Models.GeoLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Common.Interfaces.Services
{
    public interface IGeoLocationService
    {
        Task<GeoCoordinates> GetCurrentPositionAsync();
    }
}
