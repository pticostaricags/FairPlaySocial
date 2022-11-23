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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public partial class PublicUserProfileController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly UserProfileService userProfileService;

        public PublicUserProfileController(IMapper mapper, UserProfileService userProfileService)
        {
            this.mapper = mapper;
            this.userProfileService = userProfileService;
        }

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
