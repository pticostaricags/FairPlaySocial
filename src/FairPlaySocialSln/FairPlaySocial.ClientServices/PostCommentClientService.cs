using FairPlaySocial.Common.Extensions;
using FairPlaySocial.Models.Extensions;
using FairPlaySocial.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class PostCommentClientService
    {
        private readonly HttpClientService httpClientService;

        public PostCommentClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task<PostModel?> CreatePostCommentAsync(CreatePostCommentModel createPostCommentModel,
            CancellationToken cancellationToken)
        {
            var requestUrl = $"api/PostComment/CreatePostComment";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(requestUrl, createPostCommentModel, cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PostModel>(cancellationToken: cancellationToken);
            return result;
        }
    }
}
