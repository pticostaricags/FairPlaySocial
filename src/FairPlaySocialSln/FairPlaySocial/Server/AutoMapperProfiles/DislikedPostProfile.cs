using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.DislikedPost;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    public class DislikedPostProfile : Profile
    {
        public DislikedPostProfile()
        {
            CreateMap<DislikedPost, DislikedPostModel>();
            CreateMap<DislikedPostModel, DislikedPost>();
            CreateMap<CreateDislikedPostModel, DislikedPost>();
        }
    }
}
