using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.MultiplatformComponents
{
    public partial class AudibleButton
    {
        [Parameter]
        [EditorRequired]
        public string? ItemText { get; set; }
        [Parameter]
        [EditorRequired]
        public bool DisplayText { get; set; } = true;
        [Parameter]
        [EditorRequired]
        public EventCallback OnClick { get; set; }
        [Parameter]
        [EditorRequired]
        public string? CssClass { get; set; }
        [Inject]
        private ITextToSpeechService? TextToSpeechService { get; set; }

        private CancellationTokenSource? CancellationTokenSource = null;
        private string? AudioTextCue => $"Button Text: {this.ItemText}";
        private async Task OnMouseOverAsync()
        {
            this.CancellationTokenSource = new();
            await this.TextToSpeechService!
                .SpeakToDefaultSpeakersAsync(this.AudioTextCue!,
                this.CancellationTokenSource.Token);
        }

        private void OnMouseOut()
        {
            this.CancellationTokenSource!.Cancel();
        }
    }
}
