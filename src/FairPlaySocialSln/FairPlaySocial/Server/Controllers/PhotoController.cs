using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.Common.Global;
using FairPlaySocial.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Photos management.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.Admin)]
    [ControllerOfEntity(entityName: nameof(Photo), primaryKeyType: typeof(long))]
    public partial class PhotoController : ControllerBase
    {
    }
}
