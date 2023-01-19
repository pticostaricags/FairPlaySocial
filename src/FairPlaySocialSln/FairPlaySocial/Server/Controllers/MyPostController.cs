using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using FairPlaySocial.Common.Enums;
using Polly;
using FairPlaySocial.Notifications.Hubs.Post;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Handles user's posts.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyPostController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly PostService postService;
        private readonly IHubContext<PostNotificationHub, IPostNotificationHub> hubContext;
        private readonly ApplicationUserService applicationUserService;
        private readonly GroupMemberService groupMemberService;
        /// <summary>
        /// <see cref="MyPostController"/> constructor.
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/> instance.</param>
        /// <param name="currentUserProvider"><see cref="ICurrentUserProvider"/> instance.</param>
        /// <param name="applicationUserService"><see cref="ApplicationUserService"/> instance.</param>
        /// <param name="postService"><see cref="PostService"/> instance.</param>
        /// <param name="groupMemberService"><see cref="GroupMemberService"/> instance.</param>
        /// <param name="hubContext"><see cref="IHubContext"/> instance.</param>
        public MyPostController(
            IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            ApplicationUserService applicationUserService,
            PostService postService,
            GroupMemberService groupMemberService,
            IHubContext<PostNotificationHub, IPostNotificationHub> hubContext)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.postService = postService;
            this.groupMemberService = groupMemberService;
            this.hubContext = hubContext;
            this.applicationUserService = applicationUserService;
        }

        /// <summary>
        /// Creates shared post.
        /// </summary>
        /// <param name="createSharedPostModel"><see cref="CreateSharedPostModel"/> instance representing shared post to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="IActionResult"/> instance.</returns>
        /// <exception cref="CustomValidationException"></exception>
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateSharedPostAsync(
            CreateSharedPostModel createSharedPostModel,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var postEntity = await this.postService
                .GetPostByIdAsync(createSharedPostModel.CreatedFromPostId!.Value,
                trackEntities: false, cancellationToken: cancellationToken);
            if (postEntity == null)
            {
                throw new CustomValidationException("Unable to find specified post");
            }
            if (postEntity.PostVisibilityId !=
                (short)Common.Enums.PostVisibility.Public)
            {
                throw new CustomValidationException("Only public posts can be shared");
            }
            postEntity.PostId = 0;
            postEntity.CreatedFromPostId = createSharedPostModel.CreatedFromPostId;
            postEntity.Text = createSharedPostModel.Text;
            postEntity.OwnerApplicationUserId = myApplicationUserId;
            postEntity = await this.postService.CreatePostAsync(postEntity, cancellationToken: cancellationToken);
            var post = this.mapper.Map<Post, PostModel>(postEntity);
            //TODO: Consider using groups to send only to users in the "Home Feed" page
            var userEntity = await applicationUserService.GetApplicationUserByIdAsync(this.currentUserProvider.GetApplicationUserId(), trackEntities: false, cancellationToken: cancellationToken);
            post.OwnerApplicationUserFullName = userEntity.FullName;
            await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.PostNotificationModel()
            {
                PostAction = Models.Notifications.PostAction.PostCreated,
                From = userEntity.FullName,
                GroupName = null,
                Message = postEntity.Text,
                Post = post
            });
            return Ok();

        }

        /// <summary>
        /// Deletes post.
        /// </summary>
        /// <param name="postId">post id to delete.</param>
        /// <param name="fairPlaySocialDatabaseContext"><see cref="FairPlaySocialDatabaseContext"/> instance.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="IActionResult"/> instance.</returns>
        /// <exception cref="CustomValidationException"></exception>
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteMyPostAsync(
            long postId,
            [FromServices] FairPlaySocialDatabaseContext fairPlaySocialDatabaseContext,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var postEntity = await this.postService
                .GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.InverseReplyToPost)
                .Where(p => p.PostId == postId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (postEntity is null)
                throw new CustomValidationException($"Unable to find an owned post with Id: {postId}");
            if (postEntity.OwnerApplicationUserId != myApplicationUserId)
                throw new CustomValidationException($"Deleting other users posts is not allowed");

            var executionStrategy = fairPlaySocialDatabaseContext.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction = await fairPlaySocialDatabaseContext.Database
                .BeginTransactionAsync(cancellationToken: cancellationToken);
                var deletedPostKeyPhrases =
                await fairPlaySocialDatabaseContext
                .PostKeyPhrase
                .Where(p => p.PostId == postId)
                .ExecuteDeleteAsync();
                var deletedDislikedPosts =
                await fairPlaySocialDatabaseContext
                .DislikedPost
                .Where(p => p.PostId == postId)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
                var deletedlikedPosts =
                    await fairPlaySocialDatabaseContext
                    .LikedPost
                    .Where(p => p.PostId == postId)
                    .ExecuteDeleteAsync(cancellationToken: cancellationToken);
                var deletedTags =
                    await fairPlaySocialDatabaseContext
                    .PostTag
                    .Where(p => p.PostId == postId)
                    .ExecuteDeleteAsync(cancellationToken: cancellationToken);
                var deletedPostUrls =
                await fairPlaySocialDatabaseContext.PostUrl
                .Where(p => p.PostId == postId)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
                var deletedPostReplies =
                await fairPlaySocialDatabaseContext.Post.Where(p => p.RootPostId == postId)
                .OrderByDescending(p => p.PostId).ExecuteDeleteAsync(cancellationToken: cancellationToken);
                var deletedPosts =
                await fairPlaySocialDatabaseContext.Post
                .Where(p => p.PostId == postId)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
                var deletedPhotos =
                await fairPlaySocialDatabaseContext.Photo
                .Where(p => p.PhotoId == postEntity.PhotoId)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
                await transaction.CommitAsync(cancellationToken: cancellationToken);
            });
            return Ok();
        }

        /// <summary>
        /// Creates a post.
        /// </summary>
        /// <param name="forbiddenUrlService"><see cref="ForbiddenUrlService"/> instance.</param>
        /// <param name="httpClient"><see cref="HttpClient"/> instance.</param>
        /// <param name="textAnalyticsService"><see cref="TextAnalyticsService"/> instance.</param>
        /// <param name="errorLogService"><see cref="ErrorLogService"/> instance.</param>
        /// <param name="postKeyPhraseService"><see cref="PostKeyPhraseService"/> instance.</param>
        /// <param name="createPostModel"><see cref="CreatePostModel"/> instance representing post to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="IActionResult"/> instance.</returns>
        /// <exception cref="CustomValidationException"></exception>
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateMyPostAsync(
            [FromServices] ForbiddenUrlService forbiddenUrlService,
            [FromServices] HttpClient httpClient,
            [FromServices] TextAnalyticsService textAnalyticsService,
            [FromServices] ErrorLogService errorLogService,
            [FromServices] PostKeyPhraseService postKeyPhraseService,
            CreatePostModel createPostModel,
            CancellationToken cancellationToken)
        {
            if (!String.IsNullOrWhiteSpace(createPostModel.Url))
            {
                //Check https://stackoverflow.com/questions/2569851/how-to-expand-urls-in-c
                var response = await httpClient.GetAsync(createPostModel.Url, cancellationToken);
                string redirectUrl = String.Empty;
                if (
                    (response.StatusCode == System.Net.HttpStatusCode.Redirect ||
                    response.StatusCode == System.Net.HttpStatusCode.Moved) &&
                    !String.IsNullOrWhiteSpace(response.Headers.Location?.AbsoluteUri))
                {
                    redirectUrl = response.Headers.Location!.AbsoluteUri;
                }
                var isForbiddeUrl = await forbiddenUrlService
                    .GetAllForbiddenUrl(trackEntities: false, cancellationToken: cancellationToken)
                    .Where(p => EF.Functions.Like(createPostModel.Url, "%" + p.Url + "%")
                    || EF.Functions.Like(redirectUrl, "%" + p.Url + "%"))
                    .AnyAsync(cancellationToken: cancellationToken);
                if (isForbiddeUrl)
                {
                    throw new CustomValidationException($"Forbidden url: {createPostModel.Url}");
                }
            }
            if (createPostModel.GroupId != null &&
                !await this.groupMemberService
                .GetAllGroupMember(trackEntities: false,
                cancellationToken: cancellationToken)
                .Where(p => p.GroupId == createPostModel.GroupId && p.MemberApplicationUserId == this.currentUserProvider.GetApplicationUserId())
                .AnyAsync(cancellationToken: cancellationToken))
            {
                throw new CustomValidationException($"User is not a member of Group with id: {createPostModel.GroupId}");
            }
            var entity = this.mapper.Map<CreatePostModel, Post>(createPostModel);
            if (createPostModel.GroupId == null)
            {
                entity.PostVisibilityId = (short)Common.Enums.PostVisibility.Public;
            }
            entity.OwnerApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            entity.PostTypeId = (byte)Common.Enums.PostType.Post;
            entity =
            await this.postService.CreatePostAsync(entity, cancellationToken);
            var post = this.mapper.Map<Post, PostModel>(entity);
            //TODO: Consider using groups to send only to users in the "Home Feed" page
            var userEntity = await applicationUserService.GetApplicationUserByIdAsync(this.currentUserProvider.GetApplicationUserId(), trackEntities: false, cancellationToken: cancellationToken);
            post.OwnerApplicationUserFullName = userEntity.FullName;
            await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.PostNotificationModel()
            {
                PostAction = Models.Notifications.PostAction.PostCreated,
                From = userEntity.FullName,
                GroupName = null,
                Message = entity.Text,
                Post = post
            });
            try
            {
                var detectedLanguage = await textAnalyticsService
                    .DetectLanguageAsync(createPostModel!.Text!, cancellationToken);
                var postKeyPhrases = await textAnalyticsService.GetTopicsAsync(
                    createPostModel!.Text!, detectedLanguage.iso6391Name, cancellationToken);
                if (postKeyPhrases?.Count() > 0)
                {
                    await postKeyPhraseService.CreatePostKeyPhrasesAsync(
                        postId: post.PostId!.Value, postKeyPhrases,
                        cancellationToken: cancellationToken);
                }
            }
            catch (Exception ex0)
            {
                try
                {
                    await errorLogService.CreateErrorLogAsync(ex0, cancellationToken);
                }
                catch (Exception) { }
            }
            return Ok();
        }

        /// <summary>
        /// Updates post's text.
        /// </summary>
        /// <param name="postModel"><see cref="PostModel"/> instance representing the post to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="likedPostService"><see cref="LikedPostService"/> instance.</param>
        /// <param name="dislikedPostService"><see cref="DislikedPostService"/> instance.</param>
        /// <returns></returns>
        /// <exception cref="CustomValidationException"></exception>
        [HttpPut("[action]")]
        public async Task<PostModel> UpdateMyPostTextAsync(PostModel postModel,
            [FromServices] LikedPostService likedPostService,
            [FromServices] DislikedPostService dislikedPostService,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var postEntity = await this.postService
                .GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.OwnerApplicationUser)
                .Where(p => p.PostId == postModel.PostId &&
                p.OwnerApplicationUserId == myApplicationUserId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (postEntity == null)
            {
                throw new CustomValidationException($"Unable to find an owned post " +
                    $"with {nameof(postModel.PostId)}: {postModel.PostId}");
            }
            else
            {
                var likesDeleted = await likedPostService
                    .GetAllLikedPost(trackEntities: false,
                    cancellationToken: cancellationToken)
                    .Where(p => p.PostId == postModel.PostId)
                    .ExecuteDeleteAsync(cancellationToken: cancellationToken);
                var disLikesDeleted = await dislikedPostService
                    .GetAllDislikedPost(trackEntities: false,
                    cancellationToken: cancellationToken)
                    .Where(p => p.PostId == postModel.PostId)
                    .ExecuteDeleteAsync(cancellationToken: cancellationToken);
                var reSharesDeleted = await this.postService
                    .GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                    .Where(postService => postService.CreatedFromPostId == postModel.PostId)
                    .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            }
            postEntity.Text = postModel.Text;
            await this.postService.UpdatePostAsync(postEntity, cancellationToken: cancellationToken);
            await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.PostNotificationModel()
            {
                PostAction = Models.Notifications.PostAction.PostTextUpdate,
                From = postEntity.OwnerApplicationUser.FullName,
                GroupName = null,
                Message = postEntity.Text,
                Post = this.mapper.Map<Post, PostModel>(postEntity)
            });
            var result = this.mapper.Map<Post, PostModel>(postEntity);
            return result;
        }
    }
}
