using FairPlaySocial.ClientServices;
using FairPlaySocial.Common;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.ApplicationUserFollow;
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
        [Inject]
        private MyFollowClientService? MyFollowClientService { get; set; }
        private HubConnection? HubConnection { get; set; }
        private bool ShowPostAuthorModal { get; set; }
        public long? SelectedPostAuthorApplicationUserId { get; private set; }
        public ApplicationUserFollowStatusModel? MySelectedAuthorFollowStatus { get; private set; }

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
                    this.PostModels.Insert(0, model.Post!);
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

        private async Task OnPostAuthorSelectedAsync(long applicationUserId)
        {
            try
            {
                this.ShowPostAuthorModal = true;
                this.SelectedPostAuthorApplicationUserId = applicationUserId;
                this.MySelectedAuthorFollowStatus = await this.MyFollowClientService!
                    .GetMyFollowedStatusAsync(SelectedPostAuthorApplicationUserId!.Value, 
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
        }

        private void HidePostAuthorModal()
        {
            this.ShowPostAuthorModal = false;
            this.MySelectedAuthorFollowStatus = null;
            this.SelectedPostAuthorApplicationUserId = null;
        }

        private async Task OnFollowSelectedAuthorAsync()
        {
            try
            {
                await this.MyFollowClientService!
                    .FollowUserAsync(this.SelectedPostAuthorApplicationUserId!.Value,
                    CancellationToken.None);
                await OnPostAuthorSelectedAsync(this.SelectedPostAuthorApplicationUserId.Value);
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
        }

        private async Task OnUnFollowSelectedAuthorAsync()
        {
            try
            {
                await this.MyFollowClientService!
                    .UnFollowUserAsync(this.SelectedPostAuthorApplicationUserId!.Value,
                    base.CancellationToken);
                await OnPostAuthorSelectedAsync(this.SelectedPostAuthorApplicationUserId.Value);
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
        }
    }
}
