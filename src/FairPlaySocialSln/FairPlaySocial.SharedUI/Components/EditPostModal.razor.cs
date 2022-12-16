using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Post;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.SharedUI.Components
{
    public partial class EditPostModal
    {
        [Parameter]
        [EditorRequired]
        public PostModel? PostModel { get; set; }
        [Parameter]
        [EditorRequired]
        public CancellationToken CancellationToken { get; set; }
        [Parameter]
        [EditorRequired]
        public EventCallback<PostModel> OnPostUpdated { get; set; }
        [Inject]
        private MyPostClientService? MyPostClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private bool IsBusy { get; set; }
        private async Task OnValidSubmitAsync()
        {
            try
            {
                IsBusy = true;
                var result = await this.MyPostClientService!
                    .UpdateMyPostTextAsync(this.PostModel!, this.CancellationToken);
                await this.ToastService!.ShowSuccessMessageAsync("Post has been updated", this.CancellationToken);
                await this.OnPostUpdated.InvokeAsync(result);
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, this.CancellationToken!);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
