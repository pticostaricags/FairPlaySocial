using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ApplicationUserFollow;
using FairPlaySocial.Models.CustomExceptions;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyFollowController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly ApplicationUserFollowService applicationUserFollowService;

        public MyFollowController(IMapper mapper, ICurrentUserProvider currentUserProvider,
            ApplicationUserFollowService applicationUserFollowService)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.applicationUserFollowService = applicationUserFollowService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> FollowUserAsync(long userToFollowApplicationUserId, CancellationToken cancellationToken)
        {
            var myapplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var entity = new ApplicationUserFollow()
            {
                FollowerApplicationUserId = myapplicationUserId,
                FollowedApplicationUserId = userToFollowApplicationUserId
            };
            await this.applicationUserFollowService.CreateApplicationUserFollowAsync(entity, cancellationToken: cancellationToken);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UnFollowUserAsync(long userToUnFollowApplicationUserId, CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var entity = await this.applicationUserFollowService
                .GetAllApplicationUserFollow(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p => p.FollowerApplicationUserId == myApplicationUserId &&
                p.FollowedApplicationUserId == userToUnFollowApplicationUserId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (entity is null)
            {
                throw new CustomValidationException($"Your are not followin user: {userToUnFollowApplicationUserId}");
            }
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<ApplicationUserFollowStatusModel> GetMyFollowedStatusAsync(
            long userToCheckApplicationUserId, CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var entity = await this.applicationUserFollowService
                .GetAllApplicationUserFollow(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p => p.FollowerApplicationUserId == myApplicationUserId &&
                p.FollowedApplicationUserId == userToCheckApplicationUserId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            var result = new ApplicationUserFollowStatusModel()
            {
                FollowerApplicationUserId = myApplicationUserId,
                FollowedApplicationUserId = userToCheckApplicationUserId,
                IsFollowed = entity is not null
            };
            return result;
        }
    }
}
