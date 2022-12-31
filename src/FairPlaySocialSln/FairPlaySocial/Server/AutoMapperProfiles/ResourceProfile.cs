using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Localization;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    public class ResourceProfile : Profile
    {
        public ResourceProfile() 
        {
            this.CreateMap<Resource, ResourceModel>().AfterMap((source, dest) =>
            {
                if (source.Culture != null)
                {
                    dest.CultureName = source.Culture.Name;
                }
            });
        }
    }
}
