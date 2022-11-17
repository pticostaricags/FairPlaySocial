using FairPlaySocial.Models.UserPreference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class MyUserPreferenceClientService
    {
        private readonly HttpClientService _httpClientService;

        public MyUserPreferenceClientService(HttpClientService httpClientService)
        {
            this._httpClientService = httpClientService;
        }

        public async Task<UserPreferenceModel?> UpdateMyUserPreferencesAsync(
            UserPreferenceModel createUserPreferenceModel,
            CancellationToken cancellationToken)
        {
            var requestUrl = "api/MyUserPreference/UpdateMyUserPreferences";
            var authorizedHttpClient = this._httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PutAsJsonAsync(requestUrl, createUserPreferenceModel, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<UserPreferenceModel>();
            return result;
        }

        public async Task<UserPreferenceModel?> GetMyUserPreferencesAsync(CancellationToken cancellationToken)
        {
            var requestUrl = "api/MyUserPreference/GetMyUserPreferences";
            var authorizedHttpClient = this._httpClientService.CreateAuthorizedClient();
            var result = await authorizedHttpClient
                .GetFromJsonAsync<UserPreferenceModel>(requestUrl, cancellationToken);
            return result;
        }
    }
}
