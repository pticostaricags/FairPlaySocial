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
            CreateMap<ProfileVisitor, ProfileVisitorModel>()
                .AfterMap((source, dest) =>
                {
                    if (source.VisitorApplicationUser != null)
                    {
                        dest.VisitorFullName = source.VisitorApplicationUser.FullName;
                        if (source.VisitorApplicationUser.UserProfile != null)
                        {
                            dest.VisitorBio = source.VisitorApplicationUser.UserProfile.Bio;
                        }
                    }
                });
            CreateMap<CreateMyProfileVisitorModel, ProfileVisitor>();
        }
    }
}
