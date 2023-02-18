using AutoMapper;
using AutoMapper.QueryableExtensions;
using FairPlaySocial.Common.Global;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ExternalReport;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Handles external reports avaialble for user role
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles =Constants.Roles.User)]
    public class RestrictedExternalReportController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ExternalReportService externalReportService;

        /// <summary>
        /// <see cref="RestrictedExternalReportController"/> constructor
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="externalReportService"></param>
        public RestrictedExternalReportController(IMapper mapper, ExternalReportService externalReportService)
        {
            this.mapper = mapper;
            this.externalReportService = externalReportService;
        }

        /// <summary>
        /// Get all external report information
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<ExternalReportModel[]?> GetAllExternalReportsInfoAsync(CancellationToken cancellationToken)
        {
            var query = this.externalReportService.GetAllExternalReport(trackEntities: false, cancellationToken: cancellationToken);
            var result = await query.Select(p=>this.mapper.Map<ExternalReport, ExternalReportModel>(p))
                .ToArrayAsync(cancellationToken:cancellationToken);
            return result;
        }
    }
}
