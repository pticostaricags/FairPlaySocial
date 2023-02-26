using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Models.Culture;
using FairPlaySocial.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    [ClientServiceOfEntity(entityName:Constants.EntityNames.Culture, 
        primaryKeyType:typeof(int))]
    public partial class CultureClientService
    {
        public async Task<CultureModel[]?> GetAvailableCulturesAsync(CancellationToken cancellationToken)
        {
            var requestUrl = "api/Culture/GetAvailableCultures";
            var authorizedHttpClient = this._httpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(requestUrl, cancellationToken);
            await response.CustomEnsureSuccessStatusCodeAsync();
            var result = await response.Content
                .ReadFromJsonAsync<CultureModel[]>(cancellationToken:cancellationToken);
            return result;
        }
    }
}
