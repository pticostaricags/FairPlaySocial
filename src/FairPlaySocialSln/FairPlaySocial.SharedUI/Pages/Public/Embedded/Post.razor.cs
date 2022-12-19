using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.SharedUI.Pages.Public.Embedded
{
    [AllowAnonymous]
    [Route($"/Hola")]
    [Route($"/Public/Embedded/{{{nameof(PostId)}:long}}")]
    public partial class Post
    {
        [Parameter]
        public long? PostId { get; set; }
        [Inject]
        private PublicFeedClientService? PublicFeedClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private bool IsBusy { get; set; }
        private PostModel? PostModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.IsBusy = true;
                this.PostModel = await this.PublicFeedClientService!
                    .GetPostByPostIdAsync(this.PostId!.Value, base.CancellationToken);
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}
