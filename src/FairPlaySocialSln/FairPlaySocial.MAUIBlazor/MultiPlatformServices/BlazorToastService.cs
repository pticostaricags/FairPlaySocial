using FairPlaySocial.Common.Interfaces.Services;

namespace FairPlaySocial.MAUIBlazor.MultiPlatformServices
{
    public class BlazorToastService : IToastService
    {
        private readonly Blazored.Toast.Services.IToastService _blazoredToastService;

        public BlazorToastService(Blazored.Toast.Services.IToastService blazoredToastService)
        {
            this._blazoredToastService = blazoredToastService;
        }
        public Task ShowErrorMessageAsync(string message, CancellationToken cancellationToken)
        {
            this._blazoredToastService.ShowError(message);
            return Task.CompletedTask;
        }

        public Task ShowSuccessMessageAsync(string message, CancellationToken cancellationToken)
        {
            this._blazoredToastService.ShowSuccess(message);
            return Task.CompletedTask;
        }
    }
}
