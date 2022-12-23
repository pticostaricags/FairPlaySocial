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
                    if (source.PostUrl is { Count: > 0 })
                    {
                        dest.Url = source.PostUrl.First().Url;
                    }
                    if (source.PostTag is { Count: 3 })
                    {
                        dest.Tag1 = source.PostTag.ElementAt(0).Tag;
                        dest.Tag2 = source.PostTag.ElementAt(1).Tag;
                        dest.Tag3 = source.PostTag.ElementAt(2).Tag;
                    }
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
                            PhotoId = source.Photo.PhotoId,
                            AlternativeText = source.Photo.AlternativeText
                        };
                    }
                    if (source.LikedPost != null &&
                    source.LikedPost.Any())
                    {
                        dest.LikesCount = source.LikedPost.LongCount();
                    }
                    if (source.DislikedPost != null &&
                    source.DislikedPost.Any())
                    {
                        dest.DisLikesCount = source.DislikedPost.LongCount();
                    }
                });
            CreateMap<PostModel, Post>();
            CreateMap<CreatePostModel, Post>()
                .AfterMap((source, dest) =>
                {
                    if (!String.IsNullOrWhiteSpace(source.Url))
                    {
                        dest.PostUrl.Add(new PostUrl()
                        {
                            Url = source.Url
                        });
                    }
                    if (source.Photo != null)
                    {
                        dest.Photo = new Photo()
                        {
                            Filename = source.Photo.Filename,
                            ImageBytes = source.Photo.ImageBytes,
                            ImageType = source.Photo.ImageType,
                            AlternativeText = source.Photo.AlternativeText
                        };
                    }
                    dest.PostTag.Add(new PostTag()
                    {
                        Tag = source.Tag1
                    });
                    dest.PostTag.Add(new PostTag()
                    {
                        Tag = source.Tag2
                    });
                    dest.PostTag.Add(new PostTag()
                    {
                        Tag = source.Tag3
                    });

                    if (source.CreatedAtLatitude != null && source.CreatedAtLongitude != null)
                    {

                        dest.CreatedAtGeoLocation = new NetTopologySuite.Geometries.Point(
                            x: source.CreatedAtLongitude!.Value,
                            y: source.CreatedAtLatitude!.Value)
                        {
                            SRID = Common.Global.Constants.GeoCoordinates.SRID
                        };
                    }
                });
        }
    }
}
