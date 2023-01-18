using FairPlaySocial.ClientServices;
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
        private bool IsBusy { get; set; }
        private CreateUserMessageModel createUserMessageModel = new();

        protected override void OnInitialized()
        {
            this.createUserMessageModel.ToApplicationUserId = this.ToApplicationUserId;
        }

        private async Task OnValidSubmitAsync()
        {
            await UserMessageClientService!.CreateUserMessageAsync(this.createUserMessageModel,
                base.CancellationToken);
            await this.OnMessageSent.InvokeAsync();
        }
    }
}
