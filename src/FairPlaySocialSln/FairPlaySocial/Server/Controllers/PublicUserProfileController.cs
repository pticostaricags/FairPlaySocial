using AutoMapper;
using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.UserProfile;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Handles user's public profile.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public partial class PublicUserProfileController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly UserProfileService userProfileService;

        /// <summary>
        /// <see cref="PublicUserProfileController"/> constructor.
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/> instance.</param>
        /// <param name="userProfileService"><see cref="UserProfileService"/> instance.</param>
        public PublicUserProfileController(IMapper mapper, UserProfileService userProfileService)
        {
            this.mapper = mapper;
            this.userProfileService = userProfileService;
        }

        /// <summary>
        /// Gets user's public profile by user id.
        /// </summary>
        /// <param name="applicationUserId">Application user id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="UserProfileModel"/> representing user's profile.</returns>
        [Authorize(Roles = Constants.Roles.User)]
        [HttpGet("[action]")]
        public async Task<UserProfileModel>
            GetPublicUserProfileByApplicationUserIdAsync(long applicationUserId,
            CancellationToken cancellationToken)
        {
            var entity = await this.userProfileService
                .GetAllUserProfile(trackEntities: false,
                cancellationToken: cancellationToken)
                .Where(p=>p.ApplicationUserId == applicationUserId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (entity is null)
                return new UserProfileModel();
            else
            {
                var result = this.mapper.Map<UserProfile, UserProfileModel>(entity);
                return result;
            }
        }
    }
}
