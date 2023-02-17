using FairPlaySocial.Models.Extensions;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.ProfileVisitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class ProfileVisitorClientService
    {
        private readonly HttpClientService httpClientService;

        public ProfileVisitorClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task<ProfileVisitorModel?> CreateMyVisitingAsync(CreateMyProfileVisitorModel createMyVisitorProfileModel,
            CancellationToken cancellationToken)
        {
            var requestUrl = "api/ProfileVisitor/CreateMyVisiting";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(requestUrl, createMyVisitorProfileModel, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<ProfileVisitorModel>(cancellationToken: cancellationToken);
            return result;
        }

        public async Task<PagedItems<ProfileVisitorModel>?> GetMyProfileVisitorsAsync(
            PageRequestModel pageRequestModel,
            CancellationToken cancellationToken)
        {
            var requestUrl = $"api/ProfileVisitor/GetMyProfileVisitors" +
                $"?{nameof(pageRequestModel.PageNumber)}={pageRequestModel.PageNumber}";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<PagedItems<ProfileVisitorModel>>(cancellationToken: cancellationToken);
            return result;
        }
    }
}