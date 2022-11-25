using AutoMapper;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ApplicationUser;
using FairPlaySocial.Models.Post;
using Microsoft.AspNetCore.Mvc;

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
                        dest.OwnerApplicationUserId = source.OwnerApplicationUser.ApplicationUserId;
                    }
                    if (source.Photo != null)
                    {
                        dest.Photo = new Models.Photo.PhotoModel()
                        {
                            Filename = source.Photo.Filename,
                            ImageBytes = source.Photo.ImageBytes,
                            ImageType = source.Photo.ImageType,
                            PhotoId = source.Photo.PhotoId
                        };
                    }
                    if (source.LikedPost != null &&
                    source.LikedPost.Any())
                    {
                        dest.LikesCount = source.LikedPost.Count();
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
                            Filename = source.Photo.Filename,
                            ImageBytes = source.Photo.ImageBytes,
                            ImageType = source.Photo.ImageType
                        };
                    }
                });
        }
    }
}
