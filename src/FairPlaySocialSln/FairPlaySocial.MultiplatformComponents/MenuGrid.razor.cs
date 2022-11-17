using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.MultiplatformComponents
{
    public partial class MenuGrid
    {
        [EditorRequired]
        [Parameter]
        public MenuGridItem[]? MenuItems { get; set; }
        [Inject]
        private ITextToSpeechService? TextToSpeechService { get; set; }

        public class MenuGridItem
        {
            public string? Title { get; set; } = string.Empty;
            public string? TitleAudioCue => $"Menu Item: {Title}";
            public string? CssClass { get; set; } = string.Empty;
            public EventCallback OnClick { get; set; }
            public bool ShowTitleBelowIcon { get; set; } = false;
            public string? ImageSrc { get; set; }
            public string? ImageCss { get; set; }
            public bool ShowImage { get; set; }
            public CancellationTokenSource? CancellationTokenSource = null;
        }

        private void OnMouseOut(MenuGridItem menuGridItem)
        {
            menuGridItem.CancellationTokenSource!.Cancel();
        }
        private async Task OnMouseOverAsync(MenuGridItem menuGridItem)
        {
            menuGridItem.CancellationTokenSource = new CancellationTokenSource();
            await this.TextToSpeechService!.SpeakToDefaultSpeakersAsync(menuGridItem!.TitleAudioCue!,
                menuGridItem.CancellationTokenSource.Token);
        }
    }
}
