using System.ComponentModel.DataAnnotations;

namespace FairPlaySocial.Models.Photo
{
    public class CreatePhotoModel
    {
        [Required(ErrorMessage = "Please select and image")]
        [StringLength(50)]
        public string? Filename { get; set; }
        [Required]
        [StringLength(10)]
        public string? ImageType { get; set; }
        [Required]
        public byte[]? ImageBytes { get; set; }
        [Required]
        [StringLength(50)]
        public string? AlternativeText { get; set; }
    }
}
