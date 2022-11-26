using FairPlaySocial.Common.Extensions;
using FairPlaySocial.Models.DislikedPost;
using FairPlaySocial.Models.LikedPost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class MyLikedPostsClientService
    {
        private readonly HttpClientService httpClientService;

        public MyLikedPostsClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task<LikedPostModel?> LikePostAsync(CreateLikedPostModel createLikedPostModel,
            CancellationToken cancellationToken)
        {
            var requestUrl = "api/MyLikedPosts/LikePost";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(requestUrl, createLikedPostModel, cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<LikedPostModel>();
            return result;
        }

        public async Task<DislikedPostModel?> DislikePostAsync(
            CreateDislikedPostModel createDislikedPostModel,
            CancellationToken cancellationToken)
        {
            var requestUrl = "api/MyLikedPosts/DislikePost";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(requestUrl, createDislikedPostModel, cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<DislikedPostModel>();
            return result;
        }

        public async Task RemoveLikeFromPostAsync(long postId,
            CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyLikedPosts/RemoveLikeFromPost" +
                $"?{nameof(postId)}={postId}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsync(requestUri: requestUrl,
                content: null, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
        }

        public async Task RemoveDislikeFromPostAsync(long postId,
            CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyLikedPosts/RemoveDislikeFromPost" +
                $"?{nameof(postId)}={postId}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsync(requestUri: requestUrl, 
                content: null, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
        }

    }
}
