using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ApplicationUserFollow;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// <see cref="ApplicationUserFollow"/> mapping initialization.
    /// </summary>
    public class ApplicationUserFollowProfile : Profile
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ApplicationUserFollowProfile() 
        {
            CreateMap<ApplicationUserFollow, ApplicationUserFollowModel>();
            CreateMap<ApplicationUserFollowModel, ApplicationUserFollow>();
            CreateMap<CreateApplicationUserFollowModel, ApplicationUserFollow>();
        }
    }
}
