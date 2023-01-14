using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Group;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// Group mapping profile.
    /// </summary>
    public class GroupProfile : Profile
    {
        /// <summary>
        /// <see cref="GroupProfile"/> constructor.
        /// </summary>
        public GroupProfile() 
        {
            CreateMap<Group, GroupModel>();
            CreateMap<GroupModel, Group>();
            CreateMap<CreateGroupModel, Group>();
        }
    }
}
