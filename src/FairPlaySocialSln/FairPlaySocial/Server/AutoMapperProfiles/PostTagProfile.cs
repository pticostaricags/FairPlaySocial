using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.PostTag;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    public class PostTagProfile : Profile
    {
        public PostTagProfile()
        {
            CreateMap<PostTag, PostTagModel>();
            CreateMap<PostTagModel, PostTag>();
            CreateMap<CreatePostTagModel, PostTag>();
        }
    }
}
