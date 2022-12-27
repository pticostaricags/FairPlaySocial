using AutoMapper;
using FairPlaySocial.Common.Enums;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.CustomExceptions;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Notifications.Hubs;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Hosting;
using Polly;

namespace FairPlaySocial.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyPostController : ControllerBase
    {
        private IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly PostService postService;
        private readonly IHubContext<NotificationHub, INotificationHub> hubContext;
        private readonly ApplicationUserService applicationUserService;
        public MyPostController(
            IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            ApplicationUserService applicationUserService,
            PostService postService,
            IHubContext<NotificationHub, INotificationHub> hubContext)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.postService = postService;
            this.hubContext = hubContext;
            this.applicationUserService = applicationUserService;
        }

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
            await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.NotificationModel()
            {
                PostAction = Models.Notifications.PostAction.PostCreated,
                From = userEntity.FullName,
                GroupName = null,
                Message = postEntity.Text,
                Post = post
            });
            return Ok();

        }

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
                .Where(p => p.PostId == postId && p.OwnerApplicationUserId == myApplicationUserId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (postEntity is null)
                throw new CustomValidationException($"Unable to find an owned post with Id: {postId}");
            if (postEntity.InverseReplyToPost.Count > 0)
                throw new CustomValidationException($"Deleting posts with replies is not allowed");

            var executionStrategy = fairPlaySocialDatabaseContext.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction = await fairPlaySocialDatabaseContext.Database.BeginTransactionAsync();
                var deletedDislikedPosts =
                fairPlaySocialDatabaseContext
                .DislikedPost
                .Where(p => p.PostId == postId)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
                var deletedlikedPosts =
                    fairPlaySocialDatabaseContext
                    .LikedPost
                    .Where(p => p.PostId == postId)
                    .ExecuteDeleteAsync(cancellationToken: cancellationToken);
                var deletedTags =
                    fairPlaySocialDatabaseContext
                    .PostTag
                    .Where(p => p.PostId == postId)
                    .ExecuteDeleteAsync(cancellationToken: cancellationToken);
                await transaction.CommitAsync();
            });
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateMyPostAsync(
            [FromServices] ForbiddenUrlService forbiddenUrlService,
            [FromServices] HttpClient httpClient,
            CreatePostModel createPostModel,
            CancellationToken cancellationToken)
        {
            if (!String.IsNullOrWhiteSpace(createPostModel.Url))
            {
                //Check https://stackoverflow.com/questions/2569851/how-to-expand-urls-in-c
                var response = await httpClient.GetAsync(createPostModel.Url);
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
            var entity = this.mapper.Map<CreatePostModel, Post>(createPostModel);
            entity.OwnerApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            entity.PostTypeId = (byte)Common.Enums.PostType.Post;
            entity =
            await this.postService.CreatePostAsync(entity, cancellationToken);
            var post = this.mapper.Map<Post, PostModel>(entity);
            //TODO: Consider using groups to send only to users in the "Home Feed" page
            var userEntity = await applicationUserService.GetApplicationUserByIdAsync(this.currentUserProvider.GetApplicationUserId(), trackEntities: false, cancellationToken: cancellationToken);
            post.OwnerApplicationUserFullName = userEntity.FullName;
            await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.NotificationModel()
            {
                PostAction = Models.Notifications.PostAction.PostCreated,
                From = userEntity.FullName,
                GroupName = null,
                Message = entity.Text,
                Post = post
            });
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<PostModel> UpdateMyPostTextAsync(PostModel postModel,
            CancellationToken cancellationToken,
            [FromServices] LikedPostService likedPostService,
            [FromServices] DislikedPostService dislikedPostService)
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
            await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.NotificationModel()
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
