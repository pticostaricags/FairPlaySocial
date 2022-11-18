using FairPlaySocial.Common.Extensions;
using FairPlaySocial.Models.Pagination;
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

        public async Task<PagedItems<PostModel>?> GetMyHomeFeedAsync(
            PageRequestModel pageRequestModel,
            CancellationToken cancellationToken)
        {
            var requestUrl = $"api/MyFeed/GetMyHomeFeed" +
                $"?{nameof(pageRequestModel.PageNumber)}={pageRequestModel.PageNumber}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PagedItems<PostModel>>();
            return result;
        }
    }
}
