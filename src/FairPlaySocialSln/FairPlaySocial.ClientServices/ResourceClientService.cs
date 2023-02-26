using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Models.Extensions;
using FairPlaySocial.Models.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    [ClientServiceOfEntity(entityName:Constants.EntityNames.Resource,primaryKeyType:typeof(long))]
    public partial class ResourceClientService
    {
        public async Task<ResourceModel[]?> GetResourcesByCultureNameAsync(string cultureName, CancellationToken cancellationToken)
        {
            var requestUrl = $"api/Resource/GetResourcesByCultureName?{nameof(cultureName)}={cultureName}";
            var authorizedHttpClient = this._httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl, cancellationToken:cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content.ReadFromJsonAsync<ResourceModel[]>();
            return result;
        }
    }
}
