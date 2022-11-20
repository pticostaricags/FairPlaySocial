using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ApplicationUserFollow;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    public class ApplicationUserFollowProfile : Profile
    {
        public ApplicationUserFollowProfile() 
        {
            CreateMap<ApplicationUserFollow, ApplicationUserFollowModel>();
            CreateMap<ApplicationUserFollowModel, ApplicationUserFollow>();
            CreateMap<CreateApplicationUserFollowModel, ApplicationUserFollow>();
        }
    }
}
