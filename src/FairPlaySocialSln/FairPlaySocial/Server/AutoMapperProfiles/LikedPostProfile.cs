using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.LikedPost;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// <see cref="LikedPost"/> mapping profile.
    /// </summary>
    public class LikedPostProfile: Profile
    {
        /// <summary>
        /// <see cref="LikedPostProfile"/> constructor.
        /// </summary>
        public LikedPostProfile() 
        {
            CreateMap<LikedPost, LikedPostModel>();
            CreateMap<LikedPostModel, LikedPost>();
            CreateMap<CreateLikedPostModel, LikedPost>();
        }
    }
}
