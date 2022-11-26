using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.PostTag
{
    public class PostTagModel
    {
        [Required]
        public long? PostTagId { get; set; }
        [Required]
        public long? PostId { get; set; }

        [Required]
        [StringLength(50)]
        public string? Tag { get; set; }
    }
}
