using FairPlaySocial.Models.CustomExceptions;
using FairPlaySocial.Models.CustomHttpResponse;
using FairPlaySocial.Models.VisitorTracking;
using System.Net.Http.Json;
using static FairPlaySocial.Common.Global.Constants;

namespace FairPlaySocial.ClientServices
{
    public class VisitorTrackingClientService
    {
        public static Guid SessionId { get; set; }
        private long? VisitorTrackingId { get; set; }
        private HttpClientService HttpClientService { get; }
        public VisitorTrackingClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task TrackAnonymousVisitAsync(VisitorTrackingModel visitorTrackingModel,
            bool createNewSession, CancellationToken cancellationToken)
        {
            if (createNewSession)
                SessionId = Guid.NewGuid();
            visitorTrackingModel.SessionId = SessionId;
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            var response = await anonymousHttpClient
                .PostAsJsonAsync(
                ApiRoutes.VisitorTrackingController.TrackAnonymousClientInformation,
                visitorTrackingModel,
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    ProblemHttpResponse? problemHttpResponse =
                        await response.Content.ReadFromJsonAsync<ProblemHttpResponse>(cancellationToken: cancellationToken);
                    if (problemHttpResponse != null)
                        throw new CustomValidationException(problemHttpResponse.Detail!);
                    else
                        throw new CustomValidationException(response.ReasonPhrase!);
                }
                catch (Exception)
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    string reasonPhrase = response.ReasonPhrase!;
                    throw new CustomValidationException($"{reasonPhrase} - {errorText}");
                }
            }
            else
            {
                visitorTrackingModel = (await response.Content
                    .ReadFromJsonAsync<VisitorTrackingModel>(cancellationToken: cancellationToken))!;
                this.VisitorTrackingId = visitorTrackingModel.VisitorTrackingId;
            }
        }

        public async Task TrackAuthenticatedVisitAsync(VisitorTrackingModel visitorTrackingModel,
            bool createNewSession, CancellationToken cancellationToken)
        {
            if (createNewSession)
                SessionId = Guid.NewGuid();
            visitorTrackingModel.SessionId = SessionId;
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient
                .PostAsJsonAsync(
                ApiRoutes.VisitorTrackingController.TrackAuthenticatedClientInformation,
                visitorTrackingModel,
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse? problemHttpResponse =
                    await response.Content.ReadFromJsonAsync<ProblemHttpResponse>(cancellationToken: cancellationToken);
                if (problemHttpResponse != null)
                    throw new CustomValidationException(problemHttpResponse.Detail!);
                else
                    throw new CustomValidationException(response.ReasonPhrase!);
            }
            else
            {
                visitorTrackingModel = (await response.Content
                    .ReadFromJsonAsync<VisitorTrackingModel>(cancellationToken: cancellationToken))!;
                this.VisitorTrackingId = visitorTrackingModel.VisitorTrackingId;
            }
        }

        public async Task UpdateVisitTimeElapsedAsync(CancellationToken cancellationToken)
        {
            var anonymousHttpClient = HttpClientService.CreateAnonymousClient();
            var response = await anonymousHttpClient.PutAsync(
                $"{ApiRoutes.VisitorTrackingController.UpdateVisitTimeElapsed}" +
                $"?visitorTrackingId={this.VisitorTrackingId}", null,
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse? problemHttpResponse =
                    await response.Content.ReadFromJsonAsync<ProblemHttpResponse>(
                        cancellationToken: cancellationToken);
                if (problemHttpResponse != null)
                    throw new CustomValidationException(problemHttpResponse.Detail!);
                else
                    throw new CustomValidationException(response.ReasonPhrase!);
            }
        }

    }
}