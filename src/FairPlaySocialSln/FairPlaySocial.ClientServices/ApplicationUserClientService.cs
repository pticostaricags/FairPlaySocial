using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Models.ApplicationUser;
using FairPlaySocial.Models.Filtering;
using FairPlaySocial.Models.FilteringSorting;
using System.Net.Http.Json;

namespace FairPlaySocial.ClientServices
{
    [ClientServiceOfEntity(entityName: Constants.EntityNames.ApplicationUser,
        primaryKeyType: typeof(long))]
    public partial class ApplicationUserClientService
    {
        public async Task<string[]?> GetMyRolesAsync(CancellationToken cancellationToken)
        {
            var authorizedHttpClient = this._httpClientService.CreateAuthorizedClient();
            var result = await authorizedHttpClient.GetFromJsonAsync<string[]>(
                Constants.ApiRoutes.AuthController.GetMyRoles,
                cancellationToken: cancellationToken);
            return result;
        }

        public async Task<ApplicationUserModel[]?>
            GetFilteredApplicationUserAsync(FilteringSortingModel filteringSortingModel,
            CancellationToken cancellationToken)
        {
            var authorizedHttpClient = this._httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync<FilteringSortingModel>(
                "api/ApplicationUser/GetFilteredApplicationUser",filteringSortingModel,
                cancellationToken: cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ApplicationUserModel[]>(cancellationToken:cancellationToken);
            return result;
        }
    }
}
