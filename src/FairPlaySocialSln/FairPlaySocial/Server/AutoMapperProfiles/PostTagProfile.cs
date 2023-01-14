using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.PostTag;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// <see cref="PostTag"/> mapping profile.
    /// </summary>
    public class PostTagProfile : Profile
    {
        /// <summary>
        /// <see cref="PostTagProfile"/> constructor.
        /// </summary>
        public PostTagProfile()
        {
            CreateMap<PostTag, PostTagModel>();
            CreateMap<PostTagModel, PostTag>();
            CreateMap<CreatePostTagModel, PostTag>();
        }
    }
}
