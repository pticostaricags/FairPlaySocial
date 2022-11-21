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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyUserProfileController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly UserProfileService userProfileService;

        public MyUserProfileController(IMapper mapper, ICurrentUserProvider currentUserProvider, UserProfileService userProfileService)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.userProfileService = userProfileService;
        }

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
