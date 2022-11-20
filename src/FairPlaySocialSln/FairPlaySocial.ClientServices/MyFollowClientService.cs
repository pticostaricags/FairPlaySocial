using FairPlaySocial.Common.Extensions;
using FairPlaySocial.Models.ApplicationUserFollow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class MyFollowClientService
    {
        private readonly HttpClientService httpClientService;

        public MyFollowClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task FollowUserAsync(long userToFollowApplicationUserId, CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyFollow/FollowUser" +
                $"?{nameof(userToFollowApplicationUserId)}={userToFollowApplicationUserId}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsync(requestUrl, null, cancellationToken:cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
        }

        public async Task UnFollowUserAsync(long userToUnFollowApplicationUserId, CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyFollow/UnFollowUser" +
                $"?{nameof(userToUnFollowApplicationUserId)}={userToUnFollowApplicationUserId}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsync(requestUrl, null, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
        }

        public async Task<ApplicationUserFollowStatusModel?> GetMyFollowedStatusAsync(
            long userToCheckApplicationUserId, CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyFollow/GetMyFollowedStatus" +
                $"?{nameof(userToCheckApplicationUserId)}={userToCheckApplicationUserId}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl, cancellationToken:cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<ApplicationUserFollowStatusModel>(cancellationToken:cancellationToken);
            return result;
        }
    }
}
