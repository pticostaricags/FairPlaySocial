using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// In charge of exposing localization values
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        private FairPlaySocialDatabaseContext FairPlaySocialDatabaseContext { get; }

        private IMapper Mapper { get; }

        /// <summary>
        /// Initializes <see cref="LocalizationController"/>
        /// </summary>
        /// <param name="fairPlaySocialDatabaseContext"></param>
        /// <param name="mapper"></param>
        public LocalizationController(FairPlaySocialDatabaseContext fairPlaySocialDatabaseContext,
            IMapper mapper)
        {
            this.FairPlaySocialDatabaseContext = fairPlaySocialDatabaseContext;
            this.Mapper = mapper;
        }

        /// <summary>
        /// Gets all the resources.
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [OutputCache(PolicyName = Constants.Policies.OutputCaching.LocalizationResources)]
        public async Task<ResourceModel[]> GetAllResourcesAsync(CancellationToken cancellationToken)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            var result = await this.FairPlaySocialDatabaseContext.Resource
                .Include(p => p.Culture)
                .Where(p => p.Culture.Name == currentCulture.Name)
                .Select(p => this.Mapper.Map<Resource, ResourceModel>(p))
                .ToArrayAsync(cancellationToken);
            if (result.Length == 0)
                result = await this.FairPlaySocialDatabaseContext.Resource
                .Include(p => p.Culture)
                .Where(p => p.Culture.Name == "en-US")
                .Select(p => this.Mapper.Map<Resource, ResourceModel>(p))
                .ToArrayAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// Retrieve the list of all supported cultures
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [OutputCache(PolicyName = Constants.Policies.OutputCaching.SupportedCultures)]
        public async Task<CultureModel[]> GetSupportedCulturesAsync(CancellationToken cancellationToken)
        {
            return await this.FairPlaySocialDatabaseContext.Culture
                .Select(p => new CultureModel()
                {
                    Name = p.Name,
                    DisplayName = CultureInfo.GetCultureInfo(p.Name).DisplayName
                })
                .ToArrayAsync(cancellationToken: cancellationToken);
        }
    }
}
