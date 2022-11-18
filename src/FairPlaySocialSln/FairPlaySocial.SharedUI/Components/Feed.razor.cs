using FairPlaySocial.ClientServices;
using FairPlaySocial.Common;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Notifications;
using FairPlaySocial.Models.Post;
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
        public List<PostModel> PostModels { get; set; } = new List<PostModel>();
        [Parameter]
        [EditorRequired]
        public string? HubUrl { get; set; }
        [Inject]
        private HttpClientService? HttpClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private HubConnection? HubConnection { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
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
                    this.PostModels.Insert(0, new PostModel()
                    {
                        OwnerApplicationUserFullName = model.From,
                        Text = model.Message
                    });
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

        internal void Refresh()
        {
            StateHasChanged();
        }
    }
}
