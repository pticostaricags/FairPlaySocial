using FairPlaySocial.Models.CustomHttpResponse;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using System.Threading.RateLimiting;

namespace FairPlaySocial.Server
{
    /// <summary>
    /// Rate limiter extensions.
    /// </summary>
    public static class RateLimiterExtensions
    {
        /// <summary>
        /// Configures rate limiter.
        /// </summary>
        /// <param name="services">Services to configure.</param>
        /// <returns><see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection ConfigurePlatformRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                //check https://blog.maartenballiauw.be/post/2022/09/26/aspnet-core-rate-limiting-middleware.html#:~:text=Rate%20limiting%20is%20a%20way,prevent%20it%20from%20becoming%20unresponsive.
                options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
                options.OnRejected = async (context, token) =>
                {
                    var statusCode = (int)HttpStatusCode.TooManyRequests;
                    context.HttpContext.Response.StatusCode = statusCode;
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        ProblemHttpResponse problemHttpResponse = new ProblemHttpResponse()
                        {
                            Detail = $"Too many requests. Please try again after {retryAfter.TotalMinutes} minute(s). ",
                            Status = statusCode,
                            Title = "Too many requests"
                        };

                        await context.HttpContext.Response.WriteAsJsonAsync(problemHttpResponse, cancellationToken: token);
                    }
                    else
                    {
                        ProblemHttpResponse problemHttpResponse = new ProblemHttpResponse()
                        {
                            Detail = "Too many requests. Please try again later. ",
                            Status = statusCode,
                            Title = "Too many requests"
                        };
                        await context.HttpContext.Response.WriteAsJsonAsync(problemHttpResponse, cancellationToken: token);
                    }
                };
                options.AddFixedWindowLimiter(policyName: Common.Global.Constants.Policies.RateLimiting.HomeFeed,
                    options =>
                    {
                        options.AutoReplenishment = true;
                        options.PermitLimit = 60;
                        options.QueueLimit = 0;
                        options.Window = TimeSpan.FromMinutes(1);
                    });
                options.AddFixedWindowLimiter(policyName: Common.Global.Constants.Policies.RateLimiting.SendMessages,
                    options =>
                    {
                        options.AutoReplenishment = true;
                        options.PermitLimit = 10;
                        options.QueueLimit = 0;
                        options.Window = TimeSpan.FromHours(1);
                    });
            });
            return services;
        }
    }
}
