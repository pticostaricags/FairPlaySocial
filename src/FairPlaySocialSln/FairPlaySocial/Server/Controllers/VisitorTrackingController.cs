using AutoMapper;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.VisitorTracking;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Used to persis a visitor informaation
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorTrackingController : ControllerBase
    {
        private VisitorTrackingService VisitorTrackingService { get; }
        private IMapper Mapper { get; }

        /// <summary>
        /// Initialized <see cref="VisitorTrackingController"/>
        /// </summary>
        /// <param name="visitorTrackingService"></param>
        /// <param name="mapper"></param>
        public VisitorTrackingController(VisitorTrackingService visitorTrackingService,
            IMapper mapper)
        {
            this.VisitorTrackingService = visitorTrackingService;
            this.Mapper = mapper;
        }

        /// <summary>
        /// Persists the visitors information and visited page
        /// </summary>
        /// <param name="visitorTrackingModel"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<VisitorTrackingModel?> TrackAnonymousClientInformationAsync(
            VisitorTrackingModel visitorTrackingModel, CancellationToken cancellationToken)
        {
            var response = await this.VisitorTrackingService
                .TrackVisitAsync(visitorTrackingModel, cancellationToken:cancellationToken);
            if (response != null)
            {
                visitorTrackingModel = this.Mapper.Map<VisitorTracking, VisitorTrackingModel>(response);
                return visitorTrackingModel;
            }
            return null;
        }

        /// <summary>
        /// Persists the visitors information and visited page
        /// </summary>
        /// <param name="visitorTrackingModel"></param>
        /// <param name="currentUserProvider"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize]
        public async Task<VisitorTrackingModel?> TrackAuthenticatedClientInformationAsync(
            VisitorTrackingModel visitorTrackingModel, 
            [FromServices] ICurrentUserProvider currentUserProvider,
            CancellationToken cancellationToken)
        {
            var userObjectId = currentUserProvider.GetObjectId();
            visitorTrackingModel.UserAzureAdB2cObjectId = userObjectId;
            var response = await this.VisitorTrackingService
                .TrackVisitAsync(visitorTrackingModel, cancellationToken:cancellationToken);
            if (response != null)
            {
                visitorTrackingModel = this.Mapper.Map<VisitorTracking, VisitorTrackingModel>(response);
                return visitorTrackingModel;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<VisitorTrackingModel?> UpdateVisitTimeElapsedAsync(long visitorTrackingId, CancellationToken cancellationToken)
        {
            var response = await VisitorTrackingService.UpdateVisitTimeElapsedAsync(visitorTrackingId, cancellationToken);
            if (response is null)
                return null;
            var result = this.Mapper.Map<VisitorTracking, VisitorTrackingModel>(response);
            return result;
        }
    }
}
