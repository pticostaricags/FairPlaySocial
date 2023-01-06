using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Models.ClientSideErrorLog;
using FairPlaySocial.Models.CustomHttpResponse;
using System.Net.Http.Json;

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
