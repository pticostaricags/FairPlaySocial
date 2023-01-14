using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ApplicationUser;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// Application user profile mappings.
    /// </summary>
    public class ApplicationUserProfile : Profile
    {
        /// <summary>
        /// <see cref="ApplicationUserProfile"/> constructor.
        /// </summary>
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserModel>();
            CreateMap<ApplicationUserModel, ApplicationUser>();
            CreateMap<CreateApplicationUserModel, ApplicationUser>();
        }
    }
}
