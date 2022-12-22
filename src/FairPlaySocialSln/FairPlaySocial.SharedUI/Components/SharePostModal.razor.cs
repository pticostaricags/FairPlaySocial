using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Post;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.SharedUI.Components
{
    public partial class SharePostModal
    {
        [Parameter]
        [EditorRequired]
        public PostModel? PostModel { get; set; }
        [Inject]
        private IJSRuntime? JSRuntime { get; set; }
        [Inject]
        private INavigationService? NavigationService { get; set; }
        private string EmbeddedPostRelativePath => $"{Common.Global.Constants.MauiBlazorAppPages.PublicPagesRoutes.EmbeddedPost}/{this.PostModel!.PostId}";
        private string EmbeddedPostAbsolutePath => this.NavigationService!.GetAbsoluteUrl(relativePath: EmbeddedPostRelativePath);
        private string MastodonInstance { get; set; } = "mastodon";
        private async Task CopyEmbeddedPostUrlToClipboardAsync()
        {
            await this.JSRuntime!.InvokeVoidAsync("clipboardCopy.copyText", this.EmbeddedPostAbsolutePath);
        }
    }
}
