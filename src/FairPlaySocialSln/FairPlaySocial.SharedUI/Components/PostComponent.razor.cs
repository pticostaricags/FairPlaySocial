using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.ApplicationUserFollow;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Models.UserProfile;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.SharedUI.Components
{
    public partial class PostComponent
    {
        [Parameter]
        [EditorRequired]
        public PostModel? PostModel { get; set; }
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
        [Inject]
        private PostCommentClientService? PostCommentClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        [Inject]
        private INavigationService? NavigationService { get; set; }
        private bool IsBusy { get; set; }
        private bool ShowPostAuthorModal { get; set; }
        private PostModel? SelectedPostModel { get; set; }
        private ApplicationUserFollowStatusModel? MySelectedAuthorFollowStatus { get; set; }
        private UserProfileModel? MySelectedAuthorUserProfile { get; set; }
        private PostModel[]? SelectedPostHistory { get; set; }
        private bool ShowPostHistory { get; set; } = false;
        private bool ShowPostEditModal { get; set; } = false;
        private bool ShowReShareModal { get; set; } = false;
        private bool ShowPostCommentsModal { get; set; } = false;
        private CreateSharedPostModel? createSharedPostModel { get; set; } = null;
        private CreatePostCommentModel? createPostCommentModel { get; set; } = null;

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

        private void AddCommentToPost(PostModel? postModel)
        {
            this.createPostCommentModel = new()
            {
                PostId = postModel!.PostId
            };
            this.SelectedPostModel = postModel;
            this.ShowPostCommentsModal = true;
        }

        private void HidePostCommentsModal()
        {
            this.ShowPostCommentsModal = false;
            this.SelectedPostModel = null;
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

        private async Task OnValidSubmitForAddCommentToPostAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.PostCommentClientService!
                    .CreatePostCommentAsync(this.createPostCommentModel!, base.CancellationToken);
                await ToastService!
                    .ShowSuccessMessageAsync("Comment has been added", base.CancellationToken);
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

        private void NavigateToPost()
        {
            this.NavigationService!.NavigateToPost(postId: this.PostModel!.PostId!.Value);
        }
    }
}
