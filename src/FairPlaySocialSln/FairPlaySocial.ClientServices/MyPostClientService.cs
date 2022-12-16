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
    public class MyPostClientService
    {
        private HttpClientService httpClientService;

        public MyPostClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task CreateMyPostAsync(CreatePostModel createPostModel,
            CancellationToken cancellationToken)
        {
            var requestUrl = "api/MyPost/CreateMyPost";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(requestUrl, createPostModel, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
        }

        public async Task<PostModel?> UpdateMyPostTextAsync(PostModel postModel,
            CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyPost/UpdateMyPostText";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PutAsJsonAsync(requestUrl, postModel, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PostModel>();
            return result;
        }
    }
}
