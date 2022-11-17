using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FairPlaySocial.SharedUI.Components
{
    public partial class ThemeSelector
    {
        [Inject]
        private IJSRuntime? JsRuntime { get; set; }
        private async Task OnThemeSelectionChangedAsync(ChangeEventArgs changeEventArgs)
        {
            var module = await this.JsRuntime!
                .InvokeAsync<IJSObjectReference>("import",
                "./_content/FairPlaySocial.SharedUI/Components/ThemeSelector.razor.js");
            await module.InvokeVoidAsync("selectTheme", changeEventArgs.Value);
        }
    }
}
