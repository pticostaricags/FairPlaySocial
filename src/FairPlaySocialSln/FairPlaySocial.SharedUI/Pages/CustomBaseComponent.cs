using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.SharedUI.Pages
{
    public class CustomBaseComponent : ComponentBase
    {
        [Inject]
        protected IWhiteLabelingService? WhiteLabelingService { get; set; }
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        protected CancellationToken CancellationToken => this._cancellationTokenSource.Token;

        protected override async Task OnInitializedAsync()
        {
            await this.WhiteLabelingService!.LoadWhiteLabelingDataAsync();
        }
    }
}
