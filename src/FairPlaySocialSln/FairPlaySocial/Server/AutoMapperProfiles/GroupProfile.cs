using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Group;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    public class GroupProfile : Profile
    {
        public GroupProfile() 
        {
            CreateMap<Group, GroupModel>();
            CreateMap<GroupModel, Group>();
            CreateMap<CreateGroupModel, Group>();
        }
    }
}
