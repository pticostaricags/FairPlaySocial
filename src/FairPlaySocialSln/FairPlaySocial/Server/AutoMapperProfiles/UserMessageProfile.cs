using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.UserMessage;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// <see cref="UserMessage"/> mapping profile
    /// </summary>
    public class UserMessageProfile : Profile
    {
        /// <summary>
        /// <see cref="UserMessageProfile"/> constructor.
        /// </summary>
        public UserMessageProfile()
        {
            CreateMap<UserMessage, UserMessageModel>()
                .AfterMap((source, dest) => 
                {
                    if (source.FromApplicationUser != null)
                    {
                        dest.FromApplicationUserFullName = source.FromApplicationUser.FullName;
                    }
                    if (source.ToApplicationUser != null)
                    {
                        dest.ToApplicationUserFullName = source.ToApplicationUser.FullName;
                    }
                });
            CreateMap<UserMessageModel, UserMessage>();
            CreateMap<CreateUserMessageModel, UserMessage>();
        }
    }
}
