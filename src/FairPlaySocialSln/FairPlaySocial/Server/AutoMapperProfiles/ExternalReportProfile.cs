using AutoMapper;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ExternalReport;
using FairPlaySocial.Models.Group;

namespace FairPlaySocial.Server.AutoMapperProfiles
{
    /// <summary>
    /// ExternalReport mapping profile
    /// </summary>
    public class ExternalReportProfile : Profile
    {
        /// <summary>
        /// <see cref="ExternalReportProfile"/> constructor.
        /// </summary>
        public ExternalReportProfile()
        {
            CreateMap<ExternalReport, ExternalReportModel>();
            CreateMap<ExternalReportModel, ExternalReport>();
            CreateMap<CreateExternalReportModel, ExternalReport>();
        }
    }
}
