using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ApplicationUser;
using FairPlaySocial.Models.UserProfile;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// <see cref="UserProfile"/> mapping profile.
    /// </summary>
    public class UserProfileProfile : Profile
    {
        /// <summary>
        /// <see cref="UserProfileProfile"/> constructor.
        /// </summary>
        public UserProfileProfile() 
        {
            CreateMap<UserProfile, UserProfileModel>()
                .AfterMap((source, dest) => 
                {
                    if (source.ApplicationUser != null)
                    {
                        dest.FullName = source.ApplicationUser.FullName;
                    }
                });
            CreateMap<UserProfileModel, UserProfile>();
            CreateMap<CreateUserProfileModel, UserProfile>();
        }
    }
}
