using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PublicFeedController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly PostService postService;

        public PublicFeedController(
            IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            IHttpContextAccessor httpContextAccessor,
            PostService postService)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.httpContextAccessor = httpContextAccessor;
            this.postService = postService;
        }

        [HttpGet("[action]")]
        public async Task<PagedItems<PostModel>> GetUserFeedAsync(
            [FromQuery] long applicationUserId,
            [FromQuery] PageRequestModel pageRequestModel,
            [FromServices] LikedPostService likedPostService,
            [FromServices] DislikedPostService dislikedPostService,
            CancellationToken cancellationToken)
        {
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
                && p.OwnerApplicationUserId == applicationUserId
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
            foreach (var singlePost in result.Items)
            {
                singlePost.Photo!.ImageUrl =
                            $"{this.httpContextAccessor!.HttpContext!.Request.Scheme}://" +
                            $"{this.httpContextAccessor!.HttpContext!.Request.Host}/" +
                            $"api/PublicPhoto/GetPhotoByPhotoId?photoId={singlePost.Photo.PhotoId}";
            }
            return result;
        }

        [HttpGet("[action]")]
        public async Task<PostModel> GetPostByPostIdAsync(long postId, CancellationToken cancellationToken)
        {
            var postEntity = await this.postService
                .GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.OwnerApplicationUser)
                .Include(P => P.Photo)
                .Include(p => p.LikedPost)
                .Include(p => p.DislikedPost)
                .Include(p => p.PostTag)
                .Include(p => p.PostUrl)
                .Where(p => p.PostId == postId &&
                p.PostVisibilityId == (short)Common.Enums.PostVisibility.Public)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (postEntity is null)
                throw new CustomValidationException($"Unable to find post with Id: {postId}");
            var result = this.mapper.Map<Post, PostModel>(postEntity);
            result.Photo!.ImageUrl =
                            $"{this.httpContextAccessor!.HttpContext!.Request.Scheme}://" +
                            $"{this.httpContextAccessor!.HttpContext!.Request.Host}/" +
                            $"api/PublicPhoto/GetPhotoByPhotoId?photoId={result.Photo.PhotoId}";
            return result;
        }
    }
}
