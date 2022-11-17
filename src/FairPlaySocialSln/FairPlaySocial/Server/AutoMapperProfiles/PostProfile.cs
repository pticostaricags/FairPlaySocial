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
            CreateMap<Post, PostModel>()
                .AfterMap((source, dest) => 
                {
                    if (source.OwnerApplicationUser != null)
                    {
                        dest.OwnerApplicationUserFullName = source.OwnerApplicationUser.FullName;
                    }
                });
            CreateMap<PostModel, Post>();
            CreateMap<CreatePostModel, Post>();
        }
    }
}
