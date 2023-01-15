using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Handles public photos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PublicPhotoController : ControllerBase
    {
        private readonly PhotoService photoService;

        /// <summary>
        /// <see cref="PublicPhotoController"/> constructor.
        /// </summary>
        /// <param name="photoService"><see cref="PhotoService"/> instance.</param>
        public PublicPhotoController(PhotoService photoService)
        {
            this.photoService = photoService;
        }

        /// <summary>
        /// Gets photo by id.
        /// </summary>
        /// <param name="photoId">Photo id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="FileResult"/> instance.</returns>
        /// <exception cref="CustomValidationException"></exception>
        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<FileResult> GetPhotoByPhotoIdAsync(
            long photoId, CancellationToken cancellationToken)
        {
            var photoEntity = await this.photoService!
                .GetPhotoByIdAsync(photoId, trackEntities: false, cancellationToken: cancellationToken);
            if (photoEntity is null)
            {
                throw new CustomValidationException($"Unable to find photo wwith Id: {photoId}");
            }
            var result = File(photoEntity.ImageBytes,
                photoEntity.ImageType);
            return result;
        }
    }
}
