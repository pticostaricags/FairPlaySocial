using FairPlaySocial.Models.CustomExceptions;
using FairPlaySocial.Models.CustomHttpResponse;
using System.Net.Http.Json;

namespace FairPlaySocial.Models.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task CustomEnsureSuccessStatusCodeAsync(this HttpResponseMessage httpResponseMessage)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                ProblemHttpResponse? problemHttpResponse =
                    await httpResponseMessage.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                {
                    if (problemHttpResponse.Detail != null)
                    {
                        throw new CustomValidationException(problemHttpResponse.Detail!);
                    }
                    if (problemHttpResponse.Errors?.Count > 0)
                    {
                        var allValues =
                        problemHttpResponse.Errors.Values.Select(p => string.Join(",", p));
                        throw new CustomValidationException(string.Join(",", allValues));
                    }
                }
                else
                    throw new CustomValidationException(httpResponseMessage.ReasonPhrase!);
            }
        }
    }
}
