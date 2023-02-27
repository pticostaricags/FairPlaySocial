using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ApplicationUser;
using FairPlaySocial.Models.Culture;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// 
    /// </summary>
    public class CultureProfile: Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public CultureProfile()
        {
            CreateMap<Culture, CultureModel>();
            CreateMap<CultureModel, Culture>();
            CreateMap<CreateCultureModel, Culture>();
        }
    }
}
