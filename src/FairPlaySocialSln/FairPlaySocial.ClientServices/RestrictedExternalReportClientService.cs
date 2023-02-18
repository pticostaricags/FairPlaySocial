using FairPlaySocial.Models.Extensions;
using FairPlaySocial.Models.ExternalReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class RestrictedExternalReportClientService
    {
        private readonly HttpClientService httpClientService;

        public RestrictedExternalReportClientService(HttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public async Task<ExternalReportModel[]?> GetAllExternalReportsInfoAsync(CancellationToken cancellationToken)
        {
            var requestUrl = "api/RestrictedExternalReport/GetAllExternalReportsInfo";
            var authorizedHttpClient = this.httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl, cancellationToken: cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<ExternalReportModel[]>(cancellationToken: cancellationToken);
            return result;
        }
    }
}
