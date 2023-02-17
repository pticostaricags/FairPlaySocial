using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.PostTag;
using FairPlaySocial.Models.ProfileVisitor;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// /// <summary>
    /// <see cref="ProfileVisitor"/> mapping profile.
    /// </summary>
    /// </summary>
    public class ProfileVisitorProfile : Profile
    {
        /// <summary>
        /// <see cref="ProfileVisitorProfile"/> constructor.
        /// </summary>
        public ProfileVisitorProfile()
        {
            CreateMap<ProfileVisitor, ProfileVisitorModel>();
            CreateMap<CreateMyProfileVisitorModel, ProfileVisitor>();
        }
    }
}
