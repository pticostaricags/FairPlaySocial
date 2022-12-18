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
        [Inject]
        private MyFollowClientService? MyFollowClientService { get; set; }
        [Inject]
        private PublicUserProfileClientService? PublicUserProfileClientService { get; set; }
        [Inject]
        private MyLikedPostsClientService? MyLikedPostsClientService { get; set; }
        [Inject]
        private MyFeedClientService? MyFeedClientService { get; set; }
        [Inject]
        private MyPostClientService? MyPostClientService { get; set; }
        private HubConnection? HubConnection { get; set; }
        private bool ShowPostAuthorModal { get; set; }
        private PostModel? SelectedPostModel { get; set; }
        public ApplicationUserFollowStatusModel? MySelectedAuthorFollowStatus { get; private set; }
        private UserProfileModel? MySelectedAuthorUserProfile { get; set; }
        private PostModel[]? SelectedPostHistory { get; set; }
        private bool IsBusy { get; set; }
        private bool ShowPostHistory { get; set; } = false;
        private bool ShowPostEditModal { get; set; } = false;
        private bool ShowReShareModal { get; set; } = false;
        private CreateSharedPostModel? createSharedPostModel { get; set; } = null;
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
            this.SelectedPostModel = null;
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

        private async Task LikePostAsync(PostModel postModel)
        {
            try
            {
                this.IsBusy = true;
                await this.MyLikedPostsClientService!
                    .LikePostAsync(new Models.LikedPost.CreateLikedPostModel()
                    {
                        PostId = postModel.PostId
                    }, base.CancellationToken);
                postModel.IsLiked = true;
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

        private async Task DislikePostAsync(PostModel postModel)
        {
            try
            {
                this.IsBusy = true;
                await this.MyLikedPostsClientService!
                    .DislikePostAsync(new Models.DislikedPost.CreateDislikedPostModel()
                    {
                        PostId = postModel.PostId
                    }, base.CancellationToken);
                postModel.IsDisliked = true;
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

        private async Task RemoveLikeFromPostAsync(PostModel? postModel)
        {
            try
            {
                this.IsBusy = true;
                await this.MyLikedPostsClientService!
                    .RemoveLikeFromPostAsync(postModel!.PostId!.Value, base.CancellationToken);
                postModel.IsLiked = false;
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

        private async Task RemoveDislikeFromPostAsync(PostModel? postModel)
        {
            try
            {
                this.IsBusy = true;
                await this.MyLikedPostsClientService!
                    .RemoveDislikeFromPostAsync(postModel!.PostId!.Value, base.CancellationToken);
                postModel.IsDisliked = false;
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

        private async Task GetPostHistoryAsync(PostModel? postModel)
        {
            try
            {
                this.IsBusy = true;
                this.SelectedPostHistory = await this.MyFeedClientService!
                    .GetPostHistoryByPostIdAsync(postModel!.PostId!.Value, this.CancellationToken);
                this.ShowPostHistory = true;
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, this.CancellationToken);
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private void HidePostHistoryModal()
        {
            this.ShowPostHistory = false;
            this.SelectedPostHistory = null;
        }

        private void EditPost(PostModel? postModel)
        {
            this.SelectedPostModel = postModel;
            this.ShowPostEditModal = true;
        }

        private void ReSharePost(PostModel? postModel)
        {
            this.createSharedPostModel = new CreateSharedPostModel()
            {
                CreatedFromPostId = postModel!.PostId
            };
            this.SelectedPostModel = postModel;
            this.ShowReShareModal = true;
        }

        private void HidePostEditModal()
        {
            this.ShowPostEditModal = false;
            this.SelectedPostModel = null;
        }

        private void OnPostUpdated(PostModel postModel)
        {
            StateHasChanged();
            HidePostEditModal();
        }

        private void HidePostReShareModal()
        {
            this.ShowReShareModal = false;
            this.SelectedPostModel = null;
            this.createSharedPostModel = null;
        }

        private async Task OnValidSubmitForReSharePostAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.MyPostClientService!
                    .CreateSharedPostAsync(this.createSharedPostModel!, base.CancellationToken);
                await ToastService!
                    .ShowSuccessMessageAsync("Post has been Re-Shared", base.CancellationToken);
                this.HidePostReShareModal();
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
