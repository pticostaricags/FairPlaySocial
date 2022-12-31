using FairPlaySocial.Common.Global;
using FairPlaySocial.Models.ClientSideErrorLog;
using FairPlaySocial.Models.CustomExceptions;
using FairPlaySocial.Models.CustomHttpResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices
{
    public class ClientSideErrorLogClientService
    {
        private readonly HttpClientService HttpClientService;

        public ClientSideErrorLogClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task AddClientSideErrorAsync(
            CreateClientSideErrorLogModel createClientSideErrorLogModel,
            CancellationToken cancellationToken)
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            var response = await anonymousHttpClient
                .PostAsJsonAsync<CreateClientSideErrorLogModel>(
                Constants.ApiRoutes.ClientSideErrorLogController.AddClientSideError,
                createClientSideErrorLogModel,
                cancellationToken:cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var problemHttpResponse = await response.Content
                    .ReadFromJsonAsync<ProblemHttpResponse>(cancellationToken:cancellationToken);
                if (problemHttpResponse != null)
                    throw new CustomValidationException(problemHttpResponse.Detail!);
                else
                    throw new CustomValidationException(response.ReasonPhrase!);
            }
        }
    }
}
