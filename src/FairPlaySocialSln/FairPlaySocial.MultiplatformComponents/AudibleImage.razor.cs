using FairPlaySocial.Common.CustomAttributes.Localization;
using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace FairPlaySocial.MultiplatformComponents
{
    public partial class AudibleImage
    {
        [Parameter]
        [EditorRequired]
        public string? AlternativeText { get; set; }
        [Parameter]
        [EditorRequired]
        public string? Source { get; set; }
        [Inject]
        private ITextToSpeechService? TextToSpeechService { get; set; }
        [Inject]
        private IStringLocalizer<AudibleImage>? Localizer { get; set; }
        private CancellationTokenSource? CancellationTokenSource = null;
        private string AudibleTextCue => $"{Localizer![ImageAlternativeTextHintKey]}: {this.AlternativeText}";

        private async Task OnMouseOverAsync()
        {
            CancellationTokenSource = new CancellationTokenSource();
            await this.TextToSpeechService!
                .SpeakToDefaultSpeakersAsync(this.AudibleTextCue!, this.CancellationTokenSource.Token);
        }

        private async void OnMouseOut()
        {
            this.CancellationTokenSource!.Cancel();
            await this.TextToSpeechService!.CancelRunningAudioAsync();
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "Image Alternative Text")]
        public const string ImageAlternativeTextHintKey = "ImageAlternativeTextHint";
        #endregion
    }
}
