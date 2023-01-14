using AutoMapper;
using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.UserPreference;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Handles user's preferences.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public partial class MyUserPreferenceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ApplicationUserService _applicationUserService;
        private readonly UserPreferenceService _userPreferenceService;

        /// <summary>
        /// <see cref="MyUserPreferenceController"/> constructor.
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/> instance.</param>
        /// <param name="currentUserProvider"><see cref="ICurrentUserProvider"/> instance.</param>
        /// <param name="applicationUserService"><see cref="ApplicationUserService"/> instance.</param>
        /// <param name="userPreferenceService"><see cref="UserPreferenceService"/> instance.</param>
        public MyUserPreferenceController(
            IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            ApplicationUserService applicationUserService,
            UserPreferenceService userPreferenceService)
        {
            this._mapper = mapper;
            this._currentUserProvider = currentUserProvider;
            this._applicationUserService = applicationUserService;
            this._userPreferenceService = userPreferenceService;
        }

        /// <summary>
        /// Gets current user's preferences.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="UserPreferenceModel"/> instance representing user's preferences.</returns>
        [HttpGet("[action]")]
        public async Task<UserPreferenceModel> GetMyUserPreferencesAsync(CancellationToken cancellationToken)
        {
            var userObjectId = this._currentUserProvider.GetObjectId();
            var entity = await this._userPreferenceService
                .GetAllUserPreference(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.ApplicationUser)
                .Where(p => p.ApplicationUser.AzureAdB2cobjectId.ToString() ==
                userObjectId)
                .SingleOrDefaultAsync();
            if (entity is null)
            {
                return new UserPreferenceModel();
            }
            else
            {
                return this._mapper.Map<UserPreference, UserPreferenceModel>(entity);
            }
        }

        /// <summary>
        /// Updates current user's preferences.
        /// </summary>
        /// <param name="createUserPreferenceModel"><see cref="UserPreferenceModel"/> instance representing user preferences.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="UserPreferenceModel"/> instance representing updated preferences.</returns>
        [HttpPut("[action]")]
        public async Task<UserPreferenceModel> UpdateMyUserPreferencesAsync(
            UserPreferenceModel createUserPreferenceModel,
            CancellationToken cancellationToken)
        {
            var userObjectId = this._currentUserProvider.GetObjectId();
            var userEntity = await this._applicationUserService
                .GetAllApplicationUser(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p => p.AzureAdB2cobjectId.ToString() == userObjectId)
                .SingleAsync(cancellationToken:cancellationToken);
            var userPreferencesEntity =
                await this._userPreferenceService.GetAllUserPreference(
                    trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.ApplicationUser)
                .Where(p => p.ApplicationUser.ApplicationUserId ==
                userEntity.ApplicationUserId).SingleOrDefaultAsync();
            if (userPreferencesEntity is null)
            {
                userPreferencesEntity = new();
                userPreferencesEntity.ApplicationUserId = userEntity.ApplicationUserId;
                userPreferencesEntity.EnableAudibleCuesInMobile = createUserPreferenceModel.EnableAudibleCuesInMobile;
                userPreferencesEntity.EnableAudibleCuesInWeb = createUserPreferenceModel.EnableAudibleCuesInWeb;
                userPreferencesEntity = await this._userPreferenceService.CreateUserPreferenceAsync(userPreferencesEntity, cancellationToken);
            }
            else
            {
                userPreferencesEntity.EnableAudibleCuesInMobile = createUserPreferenceModel.EnableAudibleCuesInMobile;
                userPreferencesEntity.EnableAudibleCuesInWeb = createUserPreferenceModel.EnableAudibleCuesInWeb;
                userPreferencesEntity = await this._userPreferenceService.UpdateUserPreferenceAsync(userPreferencesEntity, cancellationToken);
            }
            var result = this._mapper.Map<UserPreference, UserPreferenceModel>(userPreferencesEntity);
            return result;
        }
    }
}
