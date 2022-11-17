using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Gets the name of the role assigned to the Logged In User
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<string[]> GetMyRolesAsync(
            [FromServices] ICurrentUserProvider currentUserProvider,
            [FromServices] FairPlaySocialDatabaseContext fairplaysocialDatabaseContext,
            CancellationToken cancellationToken)
        {
            List<string> result = new List<string>();
            var userAdB2CObjectId = currentUserProvider.GetObjectId();
            var rolesArray = await fairplaysocialDatabaseContext.ApplicationUserRole
                .Include(p => p.ApplicationUser)
                .Include(p => p.ApplicationRole)
                .Where(p => p.ApplicationUser.AzureAdB2cobjectId.ToString() == userAdB2CObjectId)
                .Select(p => p.ApplicationRole.Name)
                .ToArrayAsync(cancellationToken: cancellationToken);
            result.AddRange(rolesArray);
            return result.ToArray();
        }
    }
}
