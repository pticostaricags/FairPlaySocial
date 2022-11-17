using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FairPlaySocial.Client.Services
{
    public class TextToSpeechService : ITextToSpeechService
    {
        [Inject]
        private IJSRuntime? _jsRuntime { get; set; }
        public string? Locale { get; set; } = Constants.Locales.DefaultLocale;

        public TextToSpeechService(IJSRuntime? jsRuntime)
        {
            this._jsRuntime = jsRuntime;
        }
        public async Task SpeakToDefaultSpeakersAsync(string text, CancellationToken cancellationToken)
        {
            await this._jsRuntime!.InvokeVoidAsync("speakToDefaultSpeakersAsync", text, this.Locale);
        }

        public async Task CancelRunningAudioAsync()
        {
            await this._jsRuntime!.InvokeVoidAsync("cancelRunningAudio");
        }
    }
}
