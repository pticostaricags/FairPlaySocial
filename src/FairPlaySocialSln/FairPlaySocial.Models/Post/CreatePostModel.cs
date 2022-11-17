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
        public string? Text { get; set; }
    }
}
