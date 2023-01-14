using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.CustomHttpResponse;
using Microsoft.AspNetCore.Diagnostics;

namespace FairPlaySocial.Server
{
    /// <summary>
    /// Exceptions helper.
    /// </summary>
    public static class ExceptionsHelper
    {
        /// <summary>
        /// Adds the exception logging to the application.
        /// </summary>
        /// <param name="app"></param>
        public static void HandleExceptions(IApplicationBuilder app)
        {
            app.UseExceptionHandler(cfg =>
            {
                cfg.Run(async context =>
                {
                    var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();
                    var error = exceptionHandlerPathFeature?.Error;
                    if (error != null)
                    {
                        await LogExceptionAsync(context, error);
                    }
                });
            });
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="context">Current http context.</param>
        /// <param name="error">Exception to log.</param>
        /// <returns></returns>
        public static async Task LogExceptionAsync(HttpContext context, Exception error)
        {
            long? errorId;
            try
            {
                FairPlaySocialDatabaseContext fairplaysocialDatabaseContext =
                context.RequestServices.GetRequiredService<FairPlaySocialDatabaseContext>();
                ErrorLog errorLog = new()
                {
                    FullException = error.ToString(),
                    StackTrace = error.StackTrace,
                    Message = error.Message
                };
                await fairplaysocialDatabaseContext.ErrorLog.AddAsync(errorLog);
                await fairplaysocialDatabaseContext.SaveChangesAsync();
                errorId = errorLog.ErrorLogId;
            }
            catch (Exception)
            {
                throw;
            }
            ProblemHttpResponse problemHttpResponse = new();
            if (error is CustomValidationException)
            {
                problemHttpResponse.Detail = error.Message;
            }
            else
            {
                string userVisibleError = "An error ocurred.";
                if (errorId.HasValue)
                {
                    userVisibleError += $" Error code: {errorId}";
                }
                problemHttpResponse.Detail = userVisibleError;
            }
            problemHttpResponse.Status = (int)System.Net.HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync<ProblemHttpResponse>(problemHttpResponse);
        }
    }
}