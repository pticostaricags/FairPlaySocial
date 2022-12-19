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
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.SharedUI.Components
{
    public partial class Feed : IAsyncDisposable
    {
        private const string PendingNotificationsMessage = """You have pending notifications. Use the "Refresh" button to reflect latest changes""";
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
        public ApplicationUserFollowStatusModel? MySelectedAuthorFollowStatus { get; private set; }
        private bool IsBusy { get; set; }
        private Queue<NotificationModel> NotificationsQueue { get; set; } = new();
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

                this.HubConnection.On(Constants.Hubs.ReceiveMessage, (Action<NotificationModel>)((model) =>
                {
                    this.NotificationsQueue.Enqueue(model);
                    if (this.NotificationsQueue.Count == 1)
                        this.ToastService!
                        .ShowSuccessMessageAsync(PendingNotificationsMessage, base.CancellationToken)
                        .Wait();
                    StateHasChanged();
                }));
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

        private void ProcessEnqueuedNotifications()
        {
            while (this.NotificationsQueue.Count > 0)
            {
                var model = this.NotificationsQueue.Dequeue();
                ProcessNotification(model);
            }
        }

        private void ProcessNotification(NotificationModel model)
        {
            switch (model.PostAction)
            {
                case PostAction.PostCreated:
                    this.PostModels.Insert(0, model.Post!);
                    break;
                case PostAction.PostTextUpdate:
                    var matchingPost = this.PostModels.SingleOrDefault(p => p.PostId == model.Post!.PostId);
                    if (matchingPost != null)
                        matchingPost.Text = model.Post!.Text;
                    break;
                case PostAction.LikeAdded:
                case PostAction.LikeRemoved:
                    var likeChangedPost = this.PostModels.SingleOrDefault(p => p.PostId == model.Post!.PostId);
                    if (likeChangedPost != null)
                    {
                        likeChangedPost.LikesCount = model.Post!.LikesCount;
                        likeChangedPost.IsLiked = model.Post!.IsLiked;
                    }
                    break;
                case PostAction.DislikeAdded:
                case PostAction.DislikeRemoved:
                    var dislikeChangedPost = this.PostModels.SingleOrDefault(p => p.PostId == model.Post!.PostId);
                    if (dislikeChangedPost != null)
                    {
                        dislikeChangedPost.DisLikesCount = model.Post!.DisLikesCount;
                        dislikeChangedPost.IsDisliked = model.Post!.IsDisliked;
                    }
                    break;
            }
            StateHasChanged();
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
