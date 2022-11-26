using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.DislikedPost;
using FairPlaySocial.Models.LikedPost;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FairPlaySocial.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyLikedPostsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly LikedPostService likedPostService;
        private readonly DislikedPostService dislikedPostService;

        public MyLikedPostsController(IMapper mapper, 
            ICurrentUserProvider currentUserProvider, 
            LikedPostService likedPostService,
            DislikedPostService dislikedPostService)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.likedPostService = likedPostService;
            this.dislikedPostService = dislikedPostService;
        }

        [HttpPost("[action]")]
        public async Task<LikedPostModel?> LikePostAsync(CreateLikedPostModel createLikedPostModel,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var entity = this.mapper.Map<CreateLikedPostModel, LikedPost>(createLikedPostModel);
            entity.LikingApplicationUserId = myApplicationUserId;
            entity = await this.likedPostService.CreateLikedPostAsync(entity, cancellationToken);
            var result = this.mapper.Map<LikedPost, LikedPostModel>(entity);
            return result;
        }

        [HttpPost("[action]")]
        public async Task<DislikedPostModel?> DislikePostAsync(CreateDislikedPostModel createDislikedPostModel,
            CancellationToken cancellationToken)
        {
            var myApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            var entity = this.mapper.Map<CreateDislikedPostModel, DislikedPost>(createDislikedPostModel);
            entity.DislikingApplicationUserId = myApplicationUserId;
            entity = await this.dislikedPostService.CreateDislikedPostAsync(entity, cancellationToken);
            var result = this.mapper.Map<DislikedPost, DislikedPostModel>(entity);
            return result;
        }
    }
}
