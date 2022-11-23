using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.Common.Extensions;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Models.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    [ClientServiceOfEntity(entityName: Constants.EntityNames.UserProfile,
        primaryKeyType: typeof(long))]
    public partial class PublicUserProfileClientService
    {
        public async Task<UserProfileModel>
            GetPublicUserProfileByApplicationUserIdAsync(long applicationUserId,
            CancellationToken cancellationToken)
        {
            var requestUrl = $"api/PublicUserProfile/" +
                $"GetPublicUserProfileByApplicationUserId" +
                $"?{nameof(applicationUserId)}={applicationUserId}";
            var authorizedHttpClient = this._httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl, cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<UserProfileModel>();
            return result!;
        }
    }
}
