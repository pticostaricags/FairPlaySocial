using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.LikedPost;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    public class LikedPostProfile: Profile
    {
        public LikedPostProfile() 
        {
            CreateMap<LikedPost, LikedPostModel>();
            CreateMap<LikedPostModel, LikedPost>();
            CreateMap<CreateLikedPostModel, LikedPost>();
        }
    }
}
