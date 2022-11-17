using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using FairPlaySocial.Common.Interfaces.Services;

namespace FairPlaySocial.MAUIBlazor.MultiPlatformServices
{
    public class ToastService : IToastService
    {
        public async Task ShowErrorMessageAsync(string message, CancellationToken cancellationToken)
        {
            ToastDuration toastDuration = ToastDuration.Long;
            double textSize = 14;
            var toast = Toast.Make(message, toastDuration, textSize);
            await toast.Show(token: cancellationToken);
        }

        public async Task ShowSuccessMessageAsync(string message, CancellationToken cancellationToken)
        {
            ToastDuration toastDuration = ToastDuration.Short;
            double textSize = 14;
            var toast = Toast.Make(message, toastDuration, textSize);
            await toast.Show(token: cancellationToken);
        }
    }
}
