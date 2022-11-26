using FairPlaySocial.CustomValidation.CustomValidationAttributes;
using FairPlaySocial.Models.Photo;
using FairPlaySocial.Models.PostTag;
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
        private const string PATTERN_JUST_ONE_WORD = @"^\b[a-zA-Z0-9_]+\b$";
        private const string INVALID_TAG_ERROR_MESSAGE = "Tags can only contain one word composed of only letters and numbers";

        [Required]
        [StringLength(500)]
        [ProhibitHashTags(ErrorMessage = "Text cannot contain HashTags")]
        [ProhibitUrls(ErrorMessage = "Text cannot contain Urls")]
        public string? Text { get; set; }
        [Required]
        [ValidateComplexType]
        public CreatePhotoModel? Photo { get; set; }
        [Required]
        [RegularExpression(PATTERN_JUST_ONE_WORD, ErrorMessage = INVALID_TAG_ERROR_MESSAGE)]
        public string? Tag1 { get; set; }
        [Required]
        [RegularExpression(PATTERN_JUST_ONE_WORD, ErrorMessage = INVALID_TAG_ERROR_MESSAGE)]
        public string? Tag2 { get; set; }
        [Required]
        [RegularExpression(PATTERN_JUST_ONE_WORD, ErrorMessage = INVALID_TAG_ERROR_MESSAGE)]
        public string? Tag3 { get; set; }
    }
}
