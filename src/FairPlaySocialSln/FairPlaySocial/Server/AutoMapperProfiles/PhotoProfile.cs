using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Photo;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    public class PhotoProfile : Profile
    {
        public PhotoProfile()
        {
            CreateMap<Photo, PhotoModel>();
            CreateMap<PhotoModel, Photo>();
            CreateMap<CreatePhotoModel, Photo>();
        }
    }
}
