using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private CancellationTokenSource? CancellationTokenSource = null;
        private string AudibleTextCue => $"Image Alternative Text: {this.AlternativeText}";

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
    }
}
