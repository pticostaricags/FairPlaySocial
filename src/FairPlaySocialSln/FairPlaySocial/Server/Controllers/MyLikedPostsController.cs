using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.DislikedPost;
using FairPlaySocial.Models.LikedPost;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Notifications.Hubs;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Handles user's likes.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyLikedPostsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IHubContext<NotificationHub, IPostNotificationHub> hubContext;
        private readonly LikedPostService likedPostService;
        private readonly DislikedPostService dislikedPostService;
        private readonly PostService postService;

        /// <summary>
        /// <see cref="MyLikedPostsController"/> constructor.
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/> instance.</param>
        /// <param name="currentUserProvider"><see cref="ICurrentUserProvider"/> instance.</param>
        /// <param name="hubContext"><see cref="IHubContext"/> instance.</param>
        /// <param name="likedPostService"><see cref="LikedPostService"/> instance.</param>
        /// <param name="dislikedPostService"><see cref="DislikedPostService"/> instance.</param>
        /// <param name="postService"><see cref="PostService"/> instance.</param>
        public MyLikedPostsController(IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            IHubContext<NotificationHub, IPostNotificationHub> hubContext,
            LikedPostService likedPostService,
            DislikedPostService dislikedPostService,
            PostService postService)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.hubContext = hubContext;
            this.likedPostService = likedPostService;
            this.dislikedPostService = dislikedPostService;
            this.postService = postService;
        }

        /// <summary>
        /// Like post.
        /// </summary>
        /// <param name="createLikedPostModel"><see cref="CreateLikedPostModel"/> instance representing post to like.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="LikedPostModel"/> instance.</returns>
        [HttpPost("[action]")]
        public async Task<LikedPostModel?> LikePostAsync(CreateLikedPostModel createLikedPostModel,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var entity = this.mapper.Map<CreateLikedPostModel, LikedPost>(createLikedPostModel);
            entity.LikingApplicationUserId = myApplicationUserId;
            entity = await this.likedPostService.CreateLikedPostAsync(entity, cancellationToken);
            var postUpdatedEntity = this.postService.
                GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.OwnerApplicationUser)
                .Include(p => p.LikedPost)
                .Include(p => p.DislikedPost)
                .Where(p => p.PostId == createLikedPostModel.PostId)
                .First();
            await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.PostNotificationModel()
            {
                PostAction = Models.Notifications.PostAction.LikeAdded,
                From = postUpdatedEntity.OwnerApplicationUser.FullName,
                GroupName = null,
                Message = postUpdatedEntity.Text,
                Post = this.mapper.Map<Post, PostModel>(postUpdatedEntity)
            });
            var result = this.mapper.Map<LikedPost, LikedPostModel>(entity);
            return result;
        }

        /// <summary>
        /// Removes like from post.
        /// </summary>
        /// <param name="postId">Post id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="IActionResult"/> instance.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveLikeFromPostAsync(
            [FromQuery] long postId,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            //verify entity like is from logged in user executing the request
            var entity = await this.likedPostService
                .GetAllLikedPost(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p => p.PostId == postId && p.LikingApplicationUserId == myApplicationUserId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (entity is not null)
            {
                await this.likedPostService.DeleteLikedPostAsync(entity.LikedPostId, cancellationToken);
                var postUpdatedEntity = this.postService.
                GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.OwnerApplicationUser)
                .Include(p => p.LikedPost)
                .Include(p => p.DislikedPost)
                .Where(p => p.PostId == postId)
                .First();
                await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.PostNotificationModel()
                {
                    PostAction = Models.Notifications.PostAction.LikeRemoved,
                    From = postUpdatedEntity.OwnerApplicationUser.FullName,
                    GroupName = null,
                    Message = postUpdatedEntity.Text,
                    Post = this.mapper.Map<Post, PostModel>(postUpdatedEntity)
                });
            }
            return Ok(entity);
        }

        /// <summary>
        /// Removes dislike from post.
        /// </summary>
        /// <param name="postId">Post id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="IActionResult"/> instance.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveDislikeFromPostAsync(
            [FromQuery] long postId,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            //verify entity dislike is from logged in user executing the request
            var entity = await this.dislikedPostService
                .GetAllDislikedPost(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p => p.PostId == postId && p.DislikingApplicationUserId == myApplicationUserId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (entity is not null)
            {
                await this.dislikedPostService.DeleteDislikedPostAsync(entity.DislikedPostId, cancellationToken);
                var postUpdatedEntity = this.postService.
                GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.OwnerApplicationUser)
                .Include(p => p.LikedPost)
                .Include(p => p.DislikedPost)
                .Where(p => p.PostId == postId)
                .First();
                await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.PostNotificationModel()
                {
                    PostAction = Models.Notifications.PostAction.DislikeRemoved,
                    From = postUpdatedEntity.OwnerApplicationUser.FullName,
                    GroupName = null,
                    Message = postUpdatedEntity.Text,
                    Post = this.mapper.Map<Post, PostModel>(postUpdatedEntity)
                });
            }
            return Ok(entity);
        }

        /// <summary>
        /// Dislike post.
        /// </summary>
        /// <param name="createDislikedPostModel"><see cref="CreateDislikedPostModel"/> instance representing post to dislike.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="DislikedPostModel"/> instance.</returns>
        [HttpPost("[action]")]
        public async Task<DislikedPostModel?> DislikePostAsync(CreateDislikedPostModel createDislikedPostModel,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var entity = this.mapper.Map<CreateDislikedPostModel, DislikedPost>(createDislikedPostModel);
            entity.DislikingApplicationUserId = myApplicationUserId;
            entity = await this.dislikedPostService.CreateDislikedPostAsync(entity, cancellationToken);
            var postUpdatedEntity = this.postService.
                GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.OwnerApplicationUser)
                .Include(p => p.LikedPost)
                .Include(p => p.DislikedPost)
                .Where(p => p.PostId == createDislikedPostModel.PostId)
                .First();
            await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.PostNotificationModel()
            {
                PostAction = Models.Notifications.PostAction.DislikeAdded,
                From = postUpdatedEntity.OwnerApplicationUser.FullName,
                GroupName = null,
                Message = postUpdatedEntity.Text,
                Post = this.mapper.Map<Post, PostModel>(postUpdatedEntity)
            });
            var result = this.mapper.Map<DislikedPost, DislikedPostModel>(entity);
            return result;
        }
    }
}
