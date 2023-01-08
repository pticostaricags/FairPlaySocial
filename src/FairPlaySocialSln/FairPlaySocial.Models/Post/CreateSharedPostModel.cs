using FairPlaySocial.CustomValidation.CustomValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.Post
{
    public class CreateSharedPostModel
    {
        [Required]
        public long? CreatedFromPostId { get; set; }
        [Required]
        [StringLength(500)]
        [ProhibitHashTags(ErrorMessage = "Text cannot contain HashTags")]
        [ProhibitUrls(ErrorMessage = "Text cannot contain Urls")]
        public string? Text
        {
            get; set;
        }
        public long? GroupId { get; set; }
    }
}