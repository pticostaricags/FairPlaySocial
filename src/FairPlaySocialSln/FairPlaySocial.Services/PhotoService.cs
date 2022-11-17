using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.DataAccess.Models;

namespace FairPlaySocial.Services
{
    [ServiceOfEntity(entityName: nameof(Photo), primaryKeyType: typeof(long))]
    public partial class PhotoService
    {
    }
}
