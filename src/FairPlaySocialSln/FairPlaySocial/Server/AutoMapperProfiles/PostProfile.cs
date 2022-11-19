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
                    if (source.Photo != null)
                    {
                        dest.Photo = new Models.Photo.PhotoModel()
                        {
                            Filename=source.Photo.Filename,
                            ImageBytes= source.Photo.ImageBytes,
                            ImageType=source.Photo.ImageType,
                            PhotoId= source.Photo.PhotoId
                        };
                    }
                });
            CreateMap<PostModel, Post>();
            CreateMap<CreatePostModel, Post>()
                .AfterMap((source, dest) =>
                {
                    if (source.Photo != null)
                    {
                        dest.Photo = new Photo()
                        {
                            Filename= source.Photo.Filename,
                            ImageBytes= source.Photo.ImageBytes,
                            ImageType= source.Photo.ImageType
                        };
                    }
                });
        }
    }
}
