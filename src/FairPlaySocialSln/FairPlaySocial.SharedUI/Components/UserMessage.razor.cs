using FairPlaySocial.ClientServices;
using FairPlaySocial.Common;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Notifications;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.SharedUI.Components
{
    public partial class UserMessage
    {
        [Inject]
        private HttpClientService? HttpClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private HubConnection? HubConnection { get; set; }
        private bool IsBusy { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.IsBusy = true;
                var authorizedHttpClient = this.HttpClientService!.CreateAuthorizedClient();
                var hubUrl = $"{authorizedHttpClient.BaseAddress!.ToString()
                    .TrimEnd('/')}{Constants.Hubs.UserMessageHub}";
                var accessToken = UserState.UserContext.AccessToken;
                this.HubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl, options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(accessToken);
                    })
                    .Build();
                //TODO: Check an alternariif there is a better way of using async without disablign warnings
#pragma warning disable VSTHRD101 // Avoid unsupported async delegates
                this.HubConnection.On(Constants.Hubs.ReceiveMessage, 
                    (Action<UserMessageNotificationModel>)(async (model) =>
                {
                    await InvokeAsync(async () =>
                    {
                        await ToastService!
                        .ShowSuccessMessageAsync($"You have received a new message: {model.Message}", base.CancellationToken);
                        StateHasChanged();
                    });
                }));
#pragma warning restore VSTHRD101 // Avoid unsupported async delegates
                await this.HubConnection.StartAsync();
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
