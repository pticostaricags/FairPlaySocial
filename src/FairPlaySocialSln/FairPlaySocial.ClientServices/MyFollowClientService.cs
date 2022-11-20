using FairPlaySocial.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task UnFollowUserAsync(long userToFollowApplicationUserId, CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyFollow/UnFollowUser" +
                $"?{nameof(userToFollowApplicationUserId)}={userToFollowApplicationUserId}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsync(requestUrl, null, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
        }
    }
}
