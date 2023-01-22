using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.UserMessage;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.SharedUI.Components
{
    public partial class MessageComposer
    {
        [Parameter]
        [EditorRequired]
        public long? ToApplicationUserId { get; set; }
        [Parameter]
        [EditorRequired]
        public EventCallback OnMessageSent { get; set; }
        [Inject]
        private UserMessageClientService? UserMessageClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private bool IsBusy { get; set; }
        private readonly CreateUserMessageModel createUserMessageModel = new();

        protected override void OnInitialized()
        {
            this.createUserMessageModel.ToApplicationUserId = this.ToApplicationUserId;
        }

        private async Task OnValidSubmitAsync()
        {
            try
            {
                this.IsBusy = true;
                await UserMessageClientService!.CreateUserMessageAsync(this.createUserMessageModel,
                    base.CancellationToken);
                this.createUserMessageModel.Message = string.Empty;
                await this.OnMessageSent.InvokeAsync();
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
