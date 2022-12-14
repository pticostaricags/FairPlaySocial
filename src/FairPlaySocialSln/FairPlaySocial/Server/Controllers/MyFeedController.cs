using AutoMapper;
using FairPlaySocial.Common.Enums;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.RateLimiting;

namespace FairPlaySocial.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyFeedController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly PostService postService;
        private readonly GroupMemberService groupMemberService;

        public MyFeedController(
            IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            IHttpContextAccessor httpContextAccessor,
            PostService postService,
            GroupMemberService groupMemberService)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.httpContextAccessor = httpContextAccessor;
            this.postService = postService;
            this.groupMemberService = groupMemberService;
        }

        [HttpGet("[action]")]
        public async Task<PostModel?> GetPostByPostIdAsync(long postId, CancellationToken cancellationToken)
        {
            var postEntity = await this.postService.GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.OwnerApplicationUser)
                .Include(P => P.Photo)
                .Include(p => p.LikedPost)
                .Include(p => p.DislikedPost)
                .Include(p => p.PostTag)
                .Include(p => p.PostUrl)
                .Include(p => p.ReplyToPost)
                .Include(p => p.InverseReplyToPost)
                .ThenInclude(p => p.OwnerApplicationUser)
                .Include(p => p.InverseReplyToPost)
                .ThenInclude(p => p.InverseReplyToPost)
                .ThenInclude(p => p.OwnerApplicationUser)
                .Where(p => p.PostId == postId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (postEntity is null)
                throw new CustomValidationException($"Unable to find post with Id: {postId}");
            var result = this.mapper.Map<Post, PostModel>(postEntity);
            result.IsOwned = (result.OwnerApplicationUserId == this.currentUserProvider.GetApplicationUserId());
            result.Photo!.ImageUrl =
                            $"{this.httpContextAccessor!.HttpContext!.Request.Scheme}://" +
                            $"{this.httpContextAccessor!.HttpContext!.Request.Host}/" +
                            $"api/PublicPhoto/GetPhotoByPhotoId?photoId={result.Photo.PhotoId}";
            return result;
        }

        [HttpGet("[action]")]
        [EnableRateLimiting(Constants.Policies.RateLimiting.HomeFeed)] //Check https://blog.maartenballiauw.be/post/2022/09/26/aspnet-core-rate-limiting-middleware.html#:~:text=Rate%20limiting%20is%20a%20way,prevent%20it%20from%20becoming%20unresponsive.
        public async Task<PagedItems<PostModel>> GetMyHomeFeedAsync(
            [FromQuery] PageRequestModel pageRequestModel,
            [FromServices] LikedPostService likedPostService,
            [FromServices] DislikedPostService dislikedPostService,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var query = this.postService.GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.OwnerApplicationUser)
                .Include(P => P.Photo)
                .Include(p => p.LikedPost)
                .Include(p => p.DislikedPost)
                .Include(p => p.PostTag)
                .Include(p => p.PostUrl)
                .Include(p => p.ReplyToPost)
                .Include(p => p.InverseReplyToPost)
                .ThenInclude(p => p.OwnerApplicationUser)
                .Include(p => p.InverseReplyToPost)
                .ThenInclude(p => p.InverseReplyToPost)
                .ThenInclude(p => p.OwnerApplicationUser)
                .Where(p => p.PostVisibilityId == (short)Common.Enums.PostVisibility.Public &&
                p.PostTypeId == (byte)Common.Enums.PostType.Post &&
                p.GroupId == null
                );
            PagedItems<PostModel> result = new PagedItems<PostModel>();
            result.PageSize = Constants.Pagination.DefaultPageSize;
            result.PageNumber = pageRequestModel.PageNumber;
            result.TotalItems = await query.CountAsync(cancellationToken);
            result.TotalPages = (int)Math.Ceiling((double)result.TotalItems / Constants.Pagination.DefaultPageSize);
            result.Items = await query.OrderByDescending(p => p.PostId)
                .Skip((pageRequestModel.PageNumber!.Value - 1) * Constants.Pagination.DefaultPageSize)
                .Take(Constants.Pagination.DefaultPageSize)
                .Select(p => this.mapper.Map<Post, PostModel>(p))
                .ToArrayAsync(cancellationToken: cancellationToken);
            var resultingPostsIds = result.Items.Select(p => p.PostId).ToArray();
            var myLikedPosts = await likedPostService.GetAllLikedPost(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p => p.LikingApplicationUserId == myApplicationUserId &&
                resultingPostsIds.Contains(p.PostId))
                .ToArrayAsync(cancellationToken: cancellationToken);
            if (myLikedPosts.Any())
            {
                foreach (var singleLikedPost in myLikedPosts)
                {
                    var matchingItem = result.Items.Single(p => p.PostId == singleLikedPost.PostId);
                    matchingItem.IsLiked = true;
                }
            }

            var myDislikedPosts = await dislikedPostService.GetAllDislikedPost(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p => p.DislikingApplicationUserId == myApplicationUserId &&
                resultingPostsIds.Contains(p.PostId))
                .ToArrayAsync(cancellationToken: cancellationToken);
            if (myDislikedPosts.Any())
            {
                foreach (var singleDislikedPost in myDislikedPosts)
                {
                    var matchingItem = result.Items.Single(p => p.PostId == singleDislikedPost.PostId);
                    matchingItem.IsDisliked = true;
                }
            }
            foreach (var singlePost in result.Items)
            {
                singlePost.IsOwned = (singlePost.OwnerApplicationUserId == myApplicationUserId);
                singlePost.Photo!.ImageUrl =
                            $"{this.httpContextAccessor!.HttpContext!.Request.Scheme}://" +
                            $"{this.httpContextAccessor!.HttpContext!.Request.Host}/" +
                            $"api/PublicPhoto/GetPhotoByPhotoId?photoId={singlePost.Photo.PhotoId}";
            }
            return result;
        }

        [HttpGet("[action]")]
        public async Task<PagedItems<PostModel>> GetGroupFeedAsync(
            [FromQuery] PageRequestModel pageRequestModel,
            [FromQuery] long groupId,
            [FromServices] LikedPostService likedPostService,
            [FromServices] DislikedPostService dislikedPostService,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            if (
                !await groupMemberService
                .GetAllGroupMember(trackEntities: false,
                cancellationToken: cancellationToken)
                .Include(p => p.Group)
                .Where(p => p.GroupId == groupId &&
                (p.MemberApplicationUserId == myApplicationUserId ||
                p.Group.OwnerApplicationUserId == myApplicationUserId))
                .AnyAsync(cancellationToken: cancellationToken))
            {
                throw new CustomValidationException($"User is not a member of Group with id: {groupId}");
            }
            var query = this.postService.GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.OwnerApplicationUser)
                .Include(P => P.Photo)
                .Include(p => p.LikedPost)
                .Include(p => p.DislikedPost)
                .Include(p => p.PostTag)
                .Include(p => p.PostUrl)
                .Include(p => p.ReplyToPost)
                .Include(p => p.InverseReplyToPost)
                .ThenInclude(p => p.OwnerApplicationUser)
                .Include(p => p.InverseReplyToPost)
                .ThenInclude(p => p.InverseReplyToPost)
                .ThenInclude(p => p.OwnerApplicationUser)
                .Where(p => p.PostVisibilityId == (short)Common.Enums.PostVisibility.Public &&
                p.PostTypeId == (byte)Common.Enums.PostType.Post
                && p.GroupId == groupId
                );
            PagedItems<PostModel> result = new PagedItems<PostModel>();
            result.PageSize = Constants.Pagination.DefaultPageSize;
            result.PageNumber = pageRequestModel.PageNumber;
            result.TotalItems = await query.CountAsync(cancellationToken);
            result.TotalPages = (int)Math.Ceiling((double)result.TotalItems / Constants.Pagination.DefaultPageSize);
            result.Items = await query.OrderByDescending(p => p.PostId)
                .Skip((pageRequestModel.PageNumber!.Value - 1) * Constants.Pagination.DefaultPageSize)
                .Take(Constants.Pagination.DefaultPageSize)
                .Select(p => this.mapper.Map<Post, PostModel>(p))
                .ToArrayAsync(cancellationToken: cancellationToken);
            var resultingPostsIds = result.Items.Select(p => p.PostId).ToArray();
            var myLikedPosts = await likedPostService.GetAllLikedPost(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p => p.LikingApplicationUserId == myApplicationUserId &&
                resultingPostsIds.Contains(p.PostId))
                .ToArrayAsync(cancellationToken: cancellationToken);
            if (myLikedPosts.Any())
            {
                foreach (var singleLikedPost in myLikedPosts)
                {
                    var matchingItem = result.Items.Single(p => p.PostId == singleLikedPost.PostId);
                    matchingItem.IsLiked = true;
                }
            }

            var myDislikedPosts = await dislikedPostService.GetAllDislikedPost(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p => p.DislikingApplicationUserId == myApplicationUserId &&
                resultingPostsIds.Contains(p.PostId))
                .ToArrayAsync(cancellationToken: cancellationToken);
            if (myDislikedPosts.Any())
            {
                foreach (var singleDislikedPost in myDislikedPosts)
                {
                    var matchingItem = result.Items.Single(p => p.PostId == singleDislikedPost.PostId);
                    matchingItem.IsDisliked = true;
                }
            }
            foreach (var singlePost in result.Items)
            {
                singlePost.IsOwned = (singlePost.OwnerApplicationUserId == myApplicationUserId);
                singlePost.Photo!.ImageUrl =
                            $"{this.httpContextAccessor!.HttpContext!.Request.Scheme}://" +
                            $"{this.httpContextAccessor!.HttpContext!.Request.Host}/" +
                            $"api/PublicPhoto/GetPhotoByPhotoId?photoId={singlePost.Photo.PhotoId}";
            }
            return result;
        }

        [HttpGet("[action]")]
        public async Task<PostModel[]?> GetPostHistoryByPostIdAsync(long postId, CancellationToken cancellationToken)
        {
            var result =
                await this.postService!.GetPostHistoryByPostId(postId)!
                .Where(p => p.PostVisibilityId ==
                (short)Common.Enums.PostVisibility.Public)
                .OrderBy(p => EF.Property<DateTime>(p, nameof(PostModel.ValidFrom)))
                .Select(p => new PostModel()
                {
                    PostId = p.PostId,
                    Text = p.Text,
                    ValidFrom = EF.Property<DateTime>(p, nameof(PostModel.ValidFrom)),
                    ValidTo = EF.Property<DateTime>(p, nameof(PostModel.ValidTo)),
                    RowCreationDateTime = p.RowCreationDateTime
                })
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
