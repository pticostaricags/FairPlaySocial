using FairPlaySocial.Models.Extensions;
using FairPlaySocial.Models.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class MyGroupClientService
    {
        private readonly HttpClientService httpClientService;

        public MyGroupClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task<GroupModel?> CreateMyGroupAsync(CreateGroupModel createGroupModel, CancellationToken cancellationToken)
        {
            var requestUrl = "api/MyGroup/CreateMyGroup";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(requestUrl, createGroupModel, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<GroupModel>();
            return result;
        }

        public async Task JoinGroupAsync(long groupId, CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyGroup/JoinGroup?{nameof(groupId)}={groupId}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsync(requestUrl, null, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
        }
    }
}
