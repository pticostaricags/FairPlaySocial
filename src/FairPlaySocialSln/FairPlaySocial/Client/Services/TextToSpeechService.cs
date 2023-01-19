using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FairPlaySocial.Client.Services
{
    public class TextToSpeechService : ITextToSpeechService
    {
        [Inject]
        private IJSRuntime? JsRuntime { get; set; }
        public string? Locale { get; set; } = Constants.Locales.DefaultLocale;

        public TextToSpeechService(IJSRuntime? jsRuntime)
        {
            this.JsRuntime = jsRuntime;
        }
        public async Task SpeakToDefaultSpeakersAsync(string text, CancellationToken cancellationToken)
        {
            this.Locale =await this.JsRuntime!.InvokeAsync<string>("blazorCulture.get");
            if (Locale == "undefined")
                this.Locale = Constants.Locales.DefaultLocale;
            await this.JsRuntime!.InvokeVoidAsync("speakToDefaultSpeakersAsync", text, this.Locale);
        }

        public async Task CancelRunningAudioAsync()
        {
            await this.JsRuntime!.InvokeVoidAsync("cancelRunningAudio");
        }
    }
}
