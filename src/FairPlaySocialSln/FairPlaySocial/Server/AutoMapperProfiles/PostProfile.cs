using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ApplicationUser;
using FairPlaySocial.Models.Post;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    public class PostProfile : Profile
    {
        public PostProfile() 
        {
            CreateMap<Post, PostModel>();
            CreateMap<PostModel, Post>();
            CreateMap<CreatePostModel, Post>();
        }
    }
}
