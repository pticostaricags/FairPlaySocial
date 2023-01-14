using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.DislikedPost;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// Disliked post mappings.
    /// </summary>
    public class DislikedPostProfile : Profile
    {
        /// <summary>
        /// <see cref="DislikedPostProfile"/> constructor.
        /// </summary>
        public DislikedPostProfile()
        {
            CreateMap<DislikedPost, DislikedPostModel>();
            CreateMap<DislikedPostModel, DislikedPost>();
            CreateMap<CreateDislikedPostModel, DislikedPost>();
        }
    }
}
