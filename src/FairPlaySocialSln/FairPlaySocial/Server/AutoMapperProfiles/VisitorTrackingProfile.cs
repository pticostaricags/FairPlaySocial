using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.VisitorTracking;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    public class VisitorTrackingProfile : Profile
    {
        public VisitorTrackingProfile() 
        {
            CreateMap<VisitorTracking, VisitorTrackingModel>();
        }
    }
}
