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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyLikedPostsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IHubContext<NotificationHub, INotificationHub> hubContext;
        private readonly LikedPostService likedPostService;
        private readonly DislikedPostService dislikedPostService;
        private readonly PostService postService;

        public MyLikedPostsController(IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            IHubContext<NotificationHub, INotificationHub> hubContext,
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
            await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.NotificationModel()
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
                await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.NotificationModel()
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
                await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.NotificationModel()
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
            await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.NotificationModel()
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
