using AutoMapper;
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
            CancellationToken cancellationToken)
        {
            var query = this.postService.GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p => p.OwnerApplicationUser)
                .Include(P=>P.Photo);
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
            return result;
        }
    }
}
