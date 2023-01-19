using FairPlaySocial.ClientServices;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.Client.Shared
{
    public partial class Error
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Inject]
        private ClientSideErrorLogClientService? ClientSideErrorLogClientService { get; set; }
        private static readonly List<Exception> ExceptionsList = new();

        public async Task ProcessErrorAsync(Exception ex, 
            CancellationToken cancellationToken)
        {
            try
            {
                ExceptionsList.Add(ex);
                Logger.LogError("Error:ProcessError - Type: {Type} Message: {Message}",
                    ex.GetType(), ex.Message);
                await this.ClientSideErrorLogClientService!
                    .AddClientSideErrorAsync(
                    new Models.ClientSideErrorLog.CreateClientSideErrorLogModel()
                    {
                        FullException = ex.ToString(),
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    }, cancellationToken: cancellationToken
                );
            }
            catch
            {
                // If we cannot send the error to the server, there is nothing to do, 
                // therefore we ignore it
            }
        }

        public static List<Exception> GetExceptionsList() => ExceptionsList;
    }
}
