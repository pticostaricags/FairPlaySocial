using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class PostCommentController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly PostService postService;

        public PostCommentController(IMapper mapper, ICurrentUserProvider currentUserProvider, PostService postService)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.postService = postService;
        }

        [HttpPost("[action]")]
        public async Task<PostModel?> CreatePostCommentAsync(CreatePostCommentModel createPostCommentModel,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var postEntity = await this.postService
                .GetPostByIdAsync(createPostCommentModel.PostId!.Value, trackEntities: false, cancellationToken: cancellationToken);
            if (postEntity is null)
            {
                throw new CustomValidationException($"Unable to find post with Id: {createPostCommentModel.PostId.Value}");
            }
            if (postEntity.OwnerApplicationUserId == myApplicationUserId)
            {
                throw new CustomValidationException("Ths platform does not allow to create replies to owned posts or comments");
            }
            Post postCommentEntity = new Post()
            {
                PostTypeId = (byte)Common.Enums.PostType.Comment,
                ReplyToPostId = postEntity.PostId,
                Text = createPostCommentModel.Text,
                OwnerApplicationUserId = myApplicationUserId,
                PostVisibilityId = postEntity.PostVisibilityId,
                RootPostId= postEntity.RootPostId ?? postEntity.PostId
            };
            postCommentEntity = await this.postService.CreatePostAsync(postCommentEntity, cancellationToken: cancellationToken);
            postCommentEntity = (await this.postService
                .GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.OwnerApplicationUser)
                .Include(P => P.Photo)
                .Include(p => p.LikedPost)
                .Include(p => p.DislikedPost)
                .Include(p => p.PostTag)
                .Include(p => p.PostUrl)
                .Where(p => p.PostId == createPostCommentModel.PostId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken))!;
            var result = this.mapper.Map<Post, PostModel>(postCommentEntity);
            return result;
        }
    }
}
