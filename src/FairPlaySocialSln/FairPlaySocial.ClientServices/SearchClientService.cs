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
            var requestUrl = $"api/Search/SearchUserProfiles" +
                $"?{nameof(PageRequestModel.PageNumber)}={pageRequestModel.PageNumber}" +
                $"&{nameof(searchTerm)}={searchTerm}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient
                .PostAsJsonAsync(requestUrl, pageRequestModel, 
                cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PagedItems<UserProfileModel>>(cancellationToken: cancellationToken);
            return result;
        }

        public async Task<PagedItems<PostModel>?> SearchPostsAsync(
            PageRequestModel pageRequestModel,
            string searchTerm,
            CancellationToken cancellationToken)
        {
            var requestUrl = $"api/Search/SearchPosts" +
                $"?{nameof(PageRequestModel.PageNumber)}={pageRequestModel.PageNumber}" +
                $"&{nameof(searchTerm)}={searchTerm}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient
                .PostAsJsonAsync(requestUrl, pageRequestModel,
                cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PagedItems<PostModel>>(cancellationToken: cancellationToken);
            return result;
        }

        public async Task<PagedItems<GroupModel>?> SearchGroupsAsync(
            PageRequestModel pageRequestModel,
            string searchTerm,
            CancellationToken cancellationToken)
        {
            var requestUrl = $"api/Search/SearchGroups" +
                $"?{nameof(PageRequestModel.PageNumber)}={pageRequestModel.PageNumber}" +
                $"&{nameof(searchTerm)}={searchTerm}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient
                .PostAsJsonAsync(requestUrl, pageRequestModel,
                cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PagedItems<GroupModel>>(cancellationToken: cancellationToken);
            return result;
        }
    }
}
