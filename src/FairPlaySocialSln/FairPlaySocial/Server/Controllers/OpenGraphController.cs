using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class OpenGraphController : ControllerBase
    {
        private readonly PostService postService;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postService"></param>
        public OpenGraphController(PostService postService)
        {
            this.postService = postService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("/api/OpenGraph/Post/{postId}/OgImage")]
        public async Task<FileResult> GetOgImageAsync(
            long postId, CancellationToken cancellationToken)
        {
            var postEntity = await this.postService
                .GetAllPost(trackEntities: false, cancellationToken: cancellationToken)
                .Include(p=>p.Photo)
                .Where(p=>p.PostId == postId)
                .SingleOrDefaultAsync(cancellationToken:cancellationToken);
            if (postEntity is null)
                throw new CustomValidationException("Unable to find OgImage");
            return File(postEntity.Photo.ImageBytes, postEntity.Photo.ImageType);

        }
    }
}
