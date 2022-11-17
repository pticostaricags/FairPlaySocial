using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.UserPreference;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    public class UserPreferenceProfile : Profile
    {
        public UserPreferenceProfile()
        {
            CreateMap<UserPreference, UserPreferenceModel>();
            CreateMap<UserPreferenceModel, UserPreference>();
            CreateMap<CreateUserPreferenceModel, UserPreference>();
        }
    }
}
