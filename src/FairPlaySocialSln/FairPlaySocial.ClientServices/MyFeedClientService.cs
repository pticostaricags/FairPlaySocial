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
    public class MyFeedClientService
    {
        private readonly HttpClientService httpClientService;

        public MyFeedClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task<PostModel[]?> GetMyHomeFeedAsync(
            CancellationToken cancellationToken)
        {
            var requestUrl = "api/MyFeed/GetMyHomeFeed";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PostModel[]>();
            return result;
        }
    }
}
