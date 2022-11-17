using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

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

        public MyPostController(IMapper mapper, ICurrentUserProvider currentUserProvider, PostService postService)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.postService = postService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateMyPostAsync(CreatePostModel createPostModel,
            CancellationToken cancellationToken)
        {
            var entity = this.mapper.Map<CreatePostModel, Post>(createPostModel);
            entity.OwnerApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            await this.postService.CreatePostAsync(entity, cancellationToken);
            return Ok();
        }
    }
}
