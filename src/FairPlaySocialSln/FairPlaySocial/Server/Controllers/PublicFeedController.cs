using AutoMapper;
using FairPlaySocial.Common.Enums;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.CustomExceptions;
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
    [AllowAnonymous]
    public class PublicFeedController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly PostService postService;

        public PublicFeedController(IMapper mapper, PostService postService)
        {
            this.mapper = mapper;
            this.postService = postService;
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
            return result;
        }
    }
}
