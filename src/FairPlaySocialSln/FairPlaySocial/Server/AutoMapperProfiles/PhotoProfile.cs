using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Photo;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// <see cref="Photo"/> mapping profile.
    /// </summary>
    public class PhotoProfile : Profile
    {
        /// <summary>
        /// <see cref="PhotoProfile"/> constructor.
        /// </summary>
        public PhotoProfile()
        {
            CreateMap<Photo, PhotoModel>();
            CreateMap<PhotoModel, Photo>();
            CreateMap<CreatePhotoModel, Photo>();
        }
    }
}
