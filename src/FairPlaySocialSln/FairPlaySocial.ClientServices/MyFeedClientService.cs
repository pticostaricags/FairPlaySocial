using FairPlaySocial.Common.Extensions;
using FairPlaySocial.Models.Extensions;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class MyFeedClientService
    {
        private readonly HttpClientService httpClientService;

        public MyFeedClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task<PostModel?> GetPostByPostIdAsync(long postId, CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyFeed/GetPostByPostId" +
                $"?{nameof(postId)}={postId}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl, cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PostModel>(cancellationToken: cancellationToken);
            return result;
        }

        public async Task<PostModel[]?> GetPostHistoryByPostIdAsync(long postId, CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyFeed/GetPostHistoryByPostId" +
                $"?{nameof(postId)}={postId}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl, cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PostModel[]>(cancellationToken: cancellationToken);
            return result;
        }
        public async Task<PagedItems<PostModel>?> GetMyHomeFeedAsync(
            PageRequestModel pageRequestModel,
            CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyFeed/GetMyHomeFeed" +
                $"?{nameof(pageRequestModel.PageNumber)}={pageRequestModel.PageNumber}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl, cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PagedItems<PostModel>>(cancellationToken: cancellationToken);
            return result;
        }

        public async Task<PagedItems<PostModel>?> GetGroupFeedAsync(
            PageRequestModel pageRequestModel, long? groupId, CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyFeed/GetGroupFeed" +
                $"?{nameof(pageRequestModel.PageNumber)}={pageRequestModel.PageNumber}" +
                $"&{nameof(groupId)}={groupId}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl, cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PagedItems<PostModel>>(cancellationToken: cancellationToken);
            return result;
        }
    }
}
