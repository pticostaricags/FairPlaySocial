using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Localization;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// Resource mapping profile.
    /// </summary>
    public class ResourceProfile : Profile
    {
        /// <summary>
        /// <see cref="ResourceProfile"/> constructor.
        /// </summary>
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
