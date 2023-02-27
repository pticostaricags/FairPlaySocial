﻿using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Resource;

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
            this.CreateMap<ResourceModel, Resource>();
            this.CreateMap<CreateResourceModel, Resource>();
        }
    }
}
