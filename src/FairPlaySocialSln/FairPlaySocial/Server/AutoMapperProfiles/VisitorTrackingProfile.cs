using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.VisitorTracking;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// <see cref="VisitorTracking"/> mapping profile.
    /// </summary>
    public class VisitorTrackingProfile : Profile
    {
        /// <summary>
        /// <see cref="VisitorTrackingProfile"/> constructor.
        /// </summary>
        public VisitorTrackingProfile() 
        {
            CreateMap<VisitorTracking, VisitorTrackingModel>();
        }
    }
}
