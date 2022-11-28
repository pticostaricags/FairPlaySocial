﻿using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyFeedController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly PostService postService;

        public MyFeedController(IMapper mapper, ICurrentUserProvider currentUserProvider,
            PostService postService)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.postService = postService;
        }

        [HttpGet("[action]")]
        public async Task<PagedItems<PostModel>> GetMyHomeFeedAsync(
            [FromQuery] PageRequestModel pageRequestModel,
            [FromServices] LikedPostService likedPostService,
            [FromServices] DislikedPostService dislikedPostService,
            CancellationToken cancellationToken)
        {
            var query = this.postService.GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.OwnerApplicationUser)
                .Include(P => P.Photo)
                .Include(p => p.LikedPost)
                .Include(p=>p.PostTag)
                .Include(p=>p.PostUrl);
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
                .Where(p => p.LikingApplicationUserId == this.currentUserProvider.GetApplicationUserId() &&
                resultingPostsIds.Contains(p.PostId))
                .ToArrayAsync(cancellationToken: cancellationToken);
            if (myLikedPosts.Any())
            {
                foreach (var singleLikedPost in myLikedPosts) 
                {
                    var matchingItem = result.Items.Single(p=>p.PostId== singleLikedPost.PostId);
                    matchingItem.IsLiked = true;
                }
            }

            var myDislikedPosts = await dislikedPostService.GetAllDislikedPost(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p => p.DislikingApplicationUserId == this.currentUserProvider.GetApplicationUserId() &&
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
            return result;
        }
    }
}
