using FairPlaySocial.Common.Extensions;
using FairPlaySocial.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class PublicFeedClientService
    {
        private readonly HttpClientService httpClientService;

        public PublicFeedClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task<PostModel?> GetPostByPostIdAsync(long postId, CancellationToken cancellationToken)
        {
            var requestUrl = $"api/PublicFeed/GetPostByPostId?{nameof(postId)}={postId}";
            var anonymousHttpClient = this.httpClientService.CreateAnonymousClient();
            var response = await anonymousHttpClient.GetAsync(requestUrl, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PostModel>(cancellationToken: cancellationToken);
            return result;
        }
    }
}
