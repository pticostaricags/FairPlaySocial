using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ProfileVisitor;
using FairPlaySocial.Server.AutoMapperProfiles;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Handles profiles visits
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class ProfileVisitorController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly ProfileVisitorService profileVisitorService;

        /// <summary>
        /// <see cref="ProfileVisitorController"/> constructor.
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/> instance.</param>
        /// <param name="currentUserProvider"><see cref="ICurrentUserProvider"/> instance.</param>
        /// <param name="profileVisitorService"><see cref="ProfileVisitorService"/> instance.</param>
        public ProfileVisitorController(IMapper mapper, ICurrentUserProvider currentUserProvider,
            ProfileVisitorService profileVisitorService)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.profileVisitorService = profileVisitorService;
        }

        /// <summary>
        /// Adds the visiting information of the logged in user
        /// </summary>
        /// <param name="createMyVisitorProfileModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<ProfileVisitorModel?> CreateMyVisitingAsync(CreateMyProfileVisitorModel createMyVisitorProfileModel,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var entity = this.mapper.Map<CreateMyProfileVisitorModel, ProfileVisitor>(createMyVisitorProfileModel);
            entity.VisitorApplicationUserId = myApplicationUserId;
            entity = await this.profileVisitorService.CreateProfileVisitorAsync(entity, cancellationToken);
            var result = this.mapper.Map<ProfileVisitor, ProfileVisitorModel>(entity);
            return result;
        }
    }
}
