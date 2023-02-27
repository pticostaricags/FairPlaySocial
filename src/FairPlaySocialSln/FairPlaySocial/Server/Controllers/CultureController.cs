using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.Common.Global;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Culture;
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
    [ControllerOfEntity(entityName: nameof(Culture), primaryKeyType: typeof(int))]
    public partial class CultureController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<CultureModel[]> GetAvailableCulturesAsync(CancellationToken cancellationToken)
        {
            var culturesInDatabaseArray = await this.CultureService
                .GetAllCulture(trackEntities: false, cancellationToken: cancellationToken)
                .Select(p => p.Name)
                .ToArrayAsync(cancellationToken: cancellationToken);
            var allCultures =
            System.Globalization.CultureInfo
            .GetCultures(System.Globalization.CultureTypes.AllCultures)
            .Where(p => !String.IsNullOrWhiteSpace(p.Name));
            var availableCultures = allCultures
                .Where(p => !culturesInDatabaseArray.Contains(p.Name))
                .ToArray();
            var result = availableCultures.Select(p => new CultureModel()
            {
                Name = p.Name
            }).OrderBy(p => p.Name).ToArray();
            return result;
        }
    }
}
