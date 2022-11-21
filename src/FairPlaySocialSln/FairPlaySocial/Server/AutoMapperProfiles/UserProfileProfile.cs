using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ApplicationUser;
using FairPlaySocial.Models.UserProfile;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    public class UserProfileProfile : Profile
    {
        public UserProfileProfile() 
        {
            CreateMap<UserProfile, UserProfileModel>();
            CreateMap<UserProfileModel, UserProfile>();
            CreateMap<CreateUserProfileModel, UserProfile>();
        }
    }
}
