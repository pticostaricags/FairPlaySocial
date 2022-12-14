using System.ComponentModel.DataAnnotations;

namespace FairPlaySocial.Models.Photo
{
    public class PhotoModel
    {
        [Required]
        public long? PhotoId { get; set; }
        [Required]
        [StringLength(50)]
        public string? AlternativeText { get; set; }
        public string? ImageUrl { get; set; }
    }
}
