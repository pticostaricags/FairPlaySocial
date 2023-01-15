using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.Common.Global;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.ApplicationUser;
using FairPlaySocial.Models.Filtering;
using FairPlaySocial.Models.FilteringSorting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Handles user management.
    /// Accessible only to the `Admin` group member users.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.Admin)]
    [ControllerOfEntity(entityName: nameof(ApplicationUser), primaryKeyType: typeof(long))]
    public partial class ApplicationUserController : ControllerBase
    {
        /// <summary>
        /// Gets application user filtered and sorted by criteria provided.
        /// </summary>
        /// <param name="filteringSortingModel">Filter and sorting criteris.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Array of <see cref="ApplicationUserModel"/> objects.</returns>
        [HttpPost("[action]")]
        public async Task<ApplicationUserModel[]?> 
            GetFilteredApplicationUserAsync([FromBody]FilteringSortingModel filteringSortingModel, 
            CancellationToken cancellationToken)
        {
            var result = await this.ApplicationUserService.GetFilteredApplicationUser(filteringSortingModel)
                .AsNoTracking()
                .Select(p=>this.mapper.Map<ApplicationUser, ApplicationUserModel>(p))
                .ToArrayAsync(cancellationToken);
            return result;
        }
    }
}
