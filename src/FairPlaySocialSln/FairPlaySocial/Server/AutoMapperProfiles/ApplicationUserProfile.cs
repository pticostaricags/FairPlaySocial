using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ApplicationUser;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserModel>();
            CreateMap<ApplicationUserModel, ApplicationUser>();
            CreateMap<CreateApplicationUserModel, ApplicationUser>();
        }
    }
}
