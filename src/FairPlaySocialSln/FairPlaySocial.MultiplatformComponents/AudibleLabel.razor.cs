using FairPlaySocial.Common.CustomAttributes.Localization;
using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.MultiplatformComponents
{
    public partial class AudibleLabel
    {
        [Parameter]
        [EditorRequired]
        public string? ItemText { get; set; }
        [Parameter]
        public bool MakeTextBold { get; set; } = false;
        [Inject]
        private ITextToSpeechService? TextToSpeechService { get; set; }
        [Inject]
        private IStringLocalizer<AudibleLabel>? Localizer { get; set; }
        private CancellationTokenSource? CancellationTokenSource = null;
        private string AudibleTextCue => $"{Localizer![LabelTextHintKey]}: {this.ItemText}";
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
        [ResourceKey(defaultValue: "Label Text")]
        public const string LabelTextHintKey = "LabelTextHint";
        #endregion Resource Keys
    }
}
