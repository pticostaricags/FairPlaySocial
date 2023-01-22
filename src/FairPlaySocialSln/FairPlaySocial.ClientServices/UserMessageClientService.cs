using FairPlaySocial.Models.Extensions;
using FairPlaySocial.Models.UserMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class UserMessageClientService
    {
        private readonly HttpClientService httpClientService;

        public UserMessageClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task<UserMessageModel?> CreateUserMessageAsync(
            CreateUserMessageModel createUserMessageModel,
            CancellationToken cancellationToken)
        {
            var requestUrl = "api/UserMessage/CreateUserMessage";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(requestUrl, createUserMessageModel,
                cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<UserMessageModel>(cancellationToken: cancellationToken);
            return result;
        }

        public async Task<UserMessageModel[]?> GetMyMessagesWithUserAsync(
            long applicationUserId, CancellationToken cancellationToken)
        {
            var requestUrl = $"api/UserMessage/GetMyMessagesWithUser" +
                $"?{nameof(applicationUserId)}={applicationUserId}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<UserMessageModel[]>(cancellationToken: cancellationToken);
            return result;
        }
    }
}
