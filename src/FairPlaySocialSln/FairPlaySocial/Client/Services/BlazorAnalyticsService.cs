using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Interfaces.Services;

namespace FairPlaySocial.Client.Services
{
    public class BlazorAnalyticsService : IAnalyticsService
    {
        private readonly ClientSideErrorLogClientService ClientSideErrorLogClientService;
        private readonly IToastService ToastService;

        public BlazorAnalyticsService(
            ClientSideErrorLogClientService clientSideErrorLogClientService,
            IToastService toastService)
        {
            this.ClientSideErrorLogClientService = clientSideErrorLogClientService;
            this.ToastService = toastService;
        }

        public void LogEvent(EventType eventType)
        {
            //TODO: All Logic to Log Events
        }

        public async void LogException(Exception ex)
        {
            try
            {
                await this.ClientSideErrorLogClientService!.AddClientSideErrorAsync(
                    new Models.ClientSideErrorLog.CreateClientSideErrorLogModel()
                    {
                        FullException = ex.ToString(),
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    }, CancellationToken.None);
            }
            catch (Exception ex1)
            {
                await this.ToastService!.ShowErrorMessageAsync(ex1.Message, CancellationToken.None);
            }
        }
    }
}
