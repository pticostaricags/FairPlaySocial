using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.Common.Global;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Resource;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.Admin)]
    [ControllerOfEntity(entityName: nameof(Resource), primaryKeyType: typeof(long))]
    public partial class ResourceController : ControllerBase
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cultureName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<ResourceModel[]?> GetResourcesByCultureNameAsync(string cultureName, CancellationToken cancellationToken)
        {
            var query = this.ResourceService
                .GetAllResource(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p=>p.Culture)
                .Where(p=>p.Culture.Name == cultureName);
            var result = await query.Select(p=>this.mapper.Map<Resource, ResourceModel>(p))
                .ToArrayAsync(cancellationToken:cancellationToken);
            return result;
        }
    }
}
