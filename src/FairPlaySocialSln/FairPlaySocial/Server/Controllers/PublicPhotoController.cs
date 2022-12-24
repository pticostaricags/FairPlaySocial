using FairPlaySocial.Models.CustomExceptions;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FairPlaySocial.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicPhotoController : ControllerBase
    {
        private readonly PhotoService photoService;

        public PublicPhotoController(PhotoService photoService)
        {
            this.photoService = photoService;
        }

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
