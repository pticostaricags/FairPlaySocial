using FairPlaySocial.Models.Extensions;
using FairPlaySocial.Models.Group;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Models.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class SearchClientService
    {
        private readonly HttpClientService httpClientService;

        public SearchClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task<PagedItems<UserProfileModel>?> SearchUserProfilesAsync(
            PageRequestModel pageRequestModel,
            string searchTerm,
            CancellationToken cancellationToken)
        {
            var requestUrl = "api/Search/SearchUserProfiles";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient
                .PostAsJsonAsync(requestUrl, pageRequestModel, 
                cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PagedItems<UserProfileModel>>();
            return result;
        }

        public async Task<PagedItems<PostModel>?> SearchPostsAsync(
            PageRequestModel pageRequestModel,
            string searchTerm,
            CancellationToken cancellationToken)
        {
            var requestUrl = "api/Search/SearchPosts";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient
                .PostAsJsonAsync(requestUrl, pageRequestModel,
                cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PagedItems<PostModel>>();
            return result;
        }

        public async Task<PagedItems<GroupModel>?> SearchGroupsAsync(
            PageRequestModel pageRequestModel,
            string searchTerm,
            CancellationToken cancellationToken)
        {
            var requestUrl = "api/Search/SearchGroups";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient
                .PostAsJsonAsync(requestUrl, pageRequestModel,
                cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PagedItems<GroupModel>>();
            return result;
        }
    }
}
