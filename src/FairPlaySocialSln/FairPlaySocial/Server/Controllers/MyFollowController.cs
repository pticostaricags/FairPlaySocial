using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ApplicationUserFollow;
using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Hanled following operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyFollowController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly ApplicationUserFollowService applicationUserFollowService;

        /// <summary>
        /// <see cref="MyFollowController"/> constructor.
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/> instance.</param>
        /// <param name="currentUserProvider"><see cref="ICurrentUserProvider"/> instance.</param>
        /// <param name="applicationUserFollowService"><see cref="ApplicationUserFollowService"/> instance.</param>
        public MyFollowController(IMapper mapper, ICurrentUserProvider currentUserProvider,
            ApplicationUserFollowService applicationUserFollowService)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.applicationUserFollowService = applicationUserFollowService;
        }

        /// <summary>
        /// Start following user.
        /// </summary>
        /// <param name="userToFollowApplicationUserId">User id to follow.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="IActionResult"/> instance.</returns>
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

        /// <summary>
        /// Stop following user.
        /// </summary>
        /// <param name="userToUnFollowApplicationUserId">User id to stop following.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="IActionResult"/> instance.</returns>
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
            else
            {
                await this.applicationUserFollowService
                    .DeleteApplicationUserFollowAsync(entity.ApplicationUserFollowId,
                    cancellationToken:cancellationToken);
            }
            return Ok();
        }

        /// <summary>
        /// Gets user following status.
        /// </summary>
        /// <param name="userToCheckApplicationUserId">User id to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="ApplicationUserFollowStatusModel"/> instance representing followint status information.</returns>
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
