using AutoMapper;
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
    /// Handles user's profile.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyUserProfileController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly UserProfileService userProfileService;

        /// <summary>
        /// <see cref="MyUserProfileController"/> constructor.
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/> instance.</param>
        /// <param name="currentUserProvider"><see cref="ICurrentUserProvider"/> instance.</param>
        /// <param name="userProfileService"><see cref="UserProfileService"/> instance.</param>
        public MyUserProfileController(IMapper mapper, ICurrentUserProvider currentUserProvider, UserProfileService userProfileService)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.userProfileService = userProfileService;
        }

        /// <summary>
        /// Gets current user's profile.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="UserProfileModel"/> instance representing user's profile.</returns>
        [HttpGet("[action]")]
        public async Task<UserProfileModel> GetMyUserProfileAsync(CancellationToken cancellationToken)
        {
            var entity = await this.userProfileService
                .GetAllUserProfile(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p=>p.ApplicationUserId == this.currentUserProvider.GetApplicationUserId())
                .SingleOrDefaultAsync(cancellationToken:cancellationToken);
            if (entity is null)
            {
                return new UserProfileModel()
                {
                };
            }
            else
            {
                var result = this.mapper.Map<UserProfile, UserProfileModel>(entity);
                return result;
            }
        }

        /// <summary>
        /// Updates current user's profile.
        /// </summary>
        /// <param name="createUserProfileModel"><see cref="CreateUserProfileModel"/> instance representing profile to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="UserProfileModel"/> instance representing updated profile.</returns>
        [HttpPost("[action]")]
        public async Task<UserProfileModel?> UpdateMyUserProfileAsync(
            CreateUserProfileModel createUserProfileModel, CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var entity = await this.userProfileService
                .GetAllUserProfile(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p => p.ApplicationUserId == myApplicationUserId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (entity != null)
            {
                entity.YouTubeNickname = createUserProfileModel.YouTubeNickname;
                entity.FacebookNickname = createUserProfileModel.FacebookNickname;
                entity.LinkedInNickname = createUserProfileModel.LinkedInNickname;
                entity.InstagramNickname = createUserProfileModel.InstagramNickname;
                entity.Bio = createUserProfileModel.Bio;
                entity.BuyMeAcoffeeNickname = createUserProfileModel.BuyMeACoffeeNickname;
                entity = await this.userProfileService.UpdateUserProfileAsync(entity, cancellationToken: cancellationToken);
            }
            else
            {
                entity = this.mapper.Map<CreateUserProfileModel, UserProfile>(createUserProfileModel);
                entity.ApplicationUserId = myApplicationUserId;
                entity = await this.userProfileService.CreateUserProfileAsync(entity, cancellationToken: cancellationToken);
            }
            var result = this.mapper.Map<UserProfile, UserProfileModel>(entity);
            return result;
        }
    }
}
