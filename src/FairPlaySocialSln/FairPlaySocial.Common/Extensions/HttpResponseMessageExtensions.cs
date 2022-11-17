using FairPlaySocial.Models.CustomExceptions;
using FairPlaySocial.Models.CustomHttpResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Common.Extensions
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
                        problemHttpResponse.Errors.Values.Select(p => String.Join(",", p));
                        throw new CustomValidationException(String.Join(",", allValues));
                    }
                }
                else
                    throw new CustomValidationException(httpResponseMessage.ReasonPhrase!);
            }
        }
    }
}
