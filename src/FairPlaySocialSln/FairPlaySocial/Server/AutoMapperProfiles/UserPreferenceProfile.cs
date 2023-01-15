using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.UserPreference;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// <see cref="UserPreference"/> mapping profile.
    /// </summary>
    public class UserPreferenceProfile : Profile
    {
        /// <summary>
        /// <see cref="UserPreferenceProfile"/> constructor.
        /// </summary>
        public UserPreferenceProfile()
        {
            CreateMap<UserPreference, UserPreferenceModel>();
            CreateMap<UserPreferenceModel, UserPreference>();
            CreateMap<CreateUserPreferenceModel, UserPreference>();
        }
    }
}
