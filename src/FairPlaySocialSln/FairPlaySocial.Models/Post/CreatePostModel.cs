using FairPlaySocial.CustomValidation.CustomValidationAttributes;
using FairPlaySocial.Models.Photo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.Post
{
    public class CreatePostModel
    {
        [Required]
        [StringLength(500)]
        [ProhibitHasTags(ErrorMessage = "Text cannot contain HashTags")]
        public string? Text { get; set; }
        [Required]
        [ValidateComplexType]
        public CreatePhotoModel? Photo { get; set; }
    }
}
