using FairPlaySocial.Common.Extensions;
using FairPlaySocial.Models.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class MyUserProfileClientService
    {
        private readonly HttpClientService httpClientService;

        public MyUserProfileClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task<UserProfileModel?> UpdateMyUserProfileAsync(
            CreateUserProfileModel createUserProfileModel, CancellationToken cancellationToken)
        {
            var requestUrl = "api/MyUserProfile/UpdateMyUserProfile";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(requestUrl, createUserProfileModel, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<UserProfileModel>();
            return result;
        }

        public async Task<UserProfileModel> GetMyUserProfileAsync(CancellationToken cancellationToken)
        {
            var requestUrl = "api/MyUserProfile/GetMyUserProfile";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<UserProfileModel>(cancellationToken: cancellationToken);
            return result!;
        }
    }
}
