using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.Common.Global;

namespace FairPlaySocial.ClientServices
{
    [ClientServiceOfEntity(entityName: Constants.EntityNames.Photo, primaryKeyType: typeof(long))]
    public partial class PhotoClientService
    {
    }
}
