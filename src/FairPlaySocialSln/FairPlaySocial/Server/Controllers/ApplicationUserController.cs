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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.Admin)]
    [ControllerOfEntity(entityName: nameof(ApplicationUser), primaryKeyType: typeof(long))]
    public partial class ApplicationUserController : ControllerBase
    {
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
