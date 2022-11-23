using FairPlaySocial.ClientServices;
using FairPlaySocial.Common;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.ApplicationUserFollow;
using FairPlaySocial.Models.Notifications;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Models.UserProfile;
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
        [Inject]
        private PublicUserProfileClientService? PublicUserProfileClientService { get; set; }
        private HubConnection? HubConnection { get; set; }
        private bool ShowPostAuthorModal { get; set; }
        private PostModel? SelectedPostModel { get; set; }
        public ApplicationUserFollowStatusModel? MySelectedAuthorFollowStatus { get; private set; }
        private UserProfileModel? MySelectedAuthorUserProfile { get; set; }
        private bool IsBusy { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.IsBusy = true;
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
            finally
            {
                this.IsBusy = false;
            }
        }

        public bool IsConnected =>
        this.HubConnection!.State == HubConnectionState.Connected;

        public override async ValueTask DisposeAsync()
        {
            await this.HubConnection!.DisposeAsync();
            await base.DisposeAsync();
        }

        private async Task OnPostAuthorSelectedAsync(PostModel selectedPostModel)
        {
            try
            {
                this.IsBusy = true;
                this.ShowPostAuthorModal = true;
                this.SelectedPostModel = selectedPostModel;
                this.MySelectedAuthorFollowStatus = await this.MyFollowClientService!
                    .GetMyFollowedStatusAsync(
                    this.SelectedPostModel.OwnerApplicationUserId!.Value,
                    CancellationToken.None);
                this.MySelectedAuthorUserProfile = 
                    await this.PublicUserProfileClientService!
                    .GetPublicUserProfileByApplicationUserIdAsync(
                        this.SelectedPostModel.OwnerApplicationUserId!.Value,
                        CancellationToken.None);
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally { this.IsBusy = false; }
        }

        private void HidePostAuthorModal()
        {
            this.ShowPostAuthorModal = false;
            this.MySelectedAuthorFollowStatus = null;
            this.SelectedPostModel= null;
        }

        private async Task OnFollowSelectedAuthorAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.MyFollowClientService!
                    .FollowUserAsync(this.SelectedPostModel!.OwnerApplicationUserId!.Value,
                    CancellationToken.None);
                await OnPostAuthorSelectedAsync(this.SelectedPostModel);
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally { this.IsBusy = false; }
        }

        private async Task OnUnFollowSelectedAuthorAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.MyFollowClientService!
                    .UnFollowUserAsync(this.SelectedPostModel!.OwnerApplicationUserId!.Value,
                    CancellationToken.None);
                await OnPostAuthorSelectedAsync(this.SelectedPostModel);
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally { this.IsBusy = false; }
        }
    }
}
