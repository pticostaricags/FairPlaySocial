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
    public partial class Feed : IAsyncDisposable
    {
        [Parameter]
        [EditorRequired]
        public List<NotificationModel>? InitialNotifications { get; set; }
        [Parameter]
        [EditorRequired]
        public string? HubUrl { get; set; }
        [Inject]
        private HttpClientService? HttpClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private HubConnection? HubConnection { get; set; }
        private List<NotificationModel> ReceivedNotifications { get; set; } = new List<NotificationModel>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.ReceivedNotifications.AddRange(InitialNotifications!);
                var authorizedHttpClient = this.HttpClientService!.CreateAuthorizedClient();
                var hubUrl = $"{authorizedHttpClient.BaseAddress!.ToString()
                    .TrimEnd('/')}{this.HubUrl}";
                var accessToken = UserState.UserContext.AccessToken;
                this.HubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl, options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(accessToken);
                    })
                    .Build();

                this.HubConnection.On<NotificationModel>(Common.Global.Constants.Hubs.ReceiveMessage, (model) =>
                {
                    ReceivedNotifications.Add(model);
                    StateHasChanged();
                });
                await this.HubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
        }

        public bool IsConnected =>
        this.HubConnection!.State == HubConnectionState.Connected;

        public override async ValueTask DisposeAsync()
        {
            await this.HubConnection!.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}
