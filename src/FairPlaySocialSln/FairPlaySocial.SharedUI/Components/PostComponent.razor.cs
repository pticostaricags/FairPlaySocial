using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.ApplicationUserFollow;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Models.UserProfile;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FairPlaySocial.SharedUI.Components
{
    public partial class PostComponent
    {

        [Parameter]
        [EditorRequired]
        public PostModel? PostModel { get; set; }
        [Parameter]
        [EditorRequired]
        public EventCallback OnPostDeleted { get; set; }
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
        private ApplicationUserFollowStatusModel? MySelectedAuthorFollowStatus { get; set; }
        private UserProfileModel? MySelectedAuthorUserProfile { get; set; }
        private PostModel[]? SelectedPostHistory { get; set; }
        private bool ShowPostHistory { get; set; } = false;
        private bool ShowPostEditModal { get; set; } = false;
        private bool ShowPostDeleteModal { get; set; } = false;
        private bool ShowReShareModal { get; set; } = false;
        private bool ShowPostCommentsModal { get; set; } = false;
        private CreateSharedPostModel? CreateSharedPostModel { get; set; } = null;
        private CreatePostCommentModel? CreatePostCommentModel { get; set; } = null;
        private bool ShowShareModal { get; set; }
        private async Task OnPostAuthorSelectedAsync()
        {
            try
            {
                this.IsBusy = true;
                this.ShowPostAuthorModal = true;
                this.MySelectedAuthorFollowStatus = await this.MyFollowClientService!
                    .GetMyFollowedStatusAsync(
                    this.PostModel!.OwnerApplicationUserId!.Value,
                    CancellationToken.None);
                this.MySelectedAuthorUserProfile =
                    await this.PublicUserProfileClientService!
                    .GetPublicUserProfileByApplicationUserIdAsync(
                        this.PostModel!.OwnerApplicationUserId!.Value,
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
        }

        private async Task OnFollowSelectedAuthorAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.MyFollowClientService!
                    .FollowUserAsync(this.PostModel!.OwnerApplicationUserId!.Value,
                    CancellationToken.None);
                await OnPostAuthorSelectedAsync();
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
                    .UnFollowUserAsync(this.PostModel!.OwnerApplicationUserId!.Value,
                    CancellationToken.None);
                await OnPostAuthorSelectedAsync();
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

        private void EditPost()
        {
            this.ShowPostEditModal = true;
        }

        private void DeletePost()
        {
            this.ShowPostDeleteModal = true;
        }

        private void ReSharePost()
        {
            this.CreateSharedPostModel = new CreateSharedPostModel()
            {
                CreatedFromPostId = this.PostModel!.PostId,
                GroupId = this.PostModel!.GroupId
            };
            this.ShowReShareModal = true;
        }

        private void AddCommentToPost(PostModel? postModel)
        {
            this.CreatePostCommentModel = new()
            {
                PostId = postModel!.PostId
            };
            this.ShowPostCommentsModal = true;
        }

        private void HidePostCommentsModal()
        {
            this.ShowPostCommentsModal = false;
        }

        private void HidePostEditModal()
        {
            this.ShowPostEditModal = false;
        }

        private void HidePostDeleteModal()
        {
            this.ShowPostDeleteModal = false;
        }

        private async Task DeletePostAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.MyPostClientService!
                    .DeleteMyPostAsync(this.PostModel!.PostId!.Value, base.CancellationToken);
                await this.ToastService!
                    .ShowSuccessMessageAsync("Post has been deleted", base.CancellationToken);
                await this.OnPostDeleted.InvokeAsync();
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

        private void OnPostUpdated(PostModel postModel)
        {
            StateHasChanged();
            HidePostEditModal();
        }

        private void HidePostReShareModal()
        {
            this.ShowReShareModal = false;
            this.CreateSharedPostModel = null;
        }

        private void HidePostShareModal()
        {
            this.ShowShareModal = false;
        }

        private async Task OnValidSubmitForReSharePostAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.MyPostClientService!
                    .CreateSharedPostAsync(this.CreateSharedPostModel!, base.CancellationToken);
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
                    .CreatePostCommentAsync(this.CreatePostCommentModel!, base.CancellationToken);
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

        private void SharePost()
        {
            this.ShowShareModal = true;
        }

        private void OnPostReplyDeleted(PostModel? postModel)
        {
            PostModel!.InverseReplyToPost =
                PostModel!.InverseReplyToPost!.Where(p => p.PostId != postModel!.PostId).ToArray();
        }
    }
}
