using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.DislikedPost
{
    public class DislikedPostModel
    {
        [Required]
        public long? DislikedPostId { get; set; }
        [Required]
        public long? PostId { get; set; }
        [Required]
        public long? DislikingApplicationUserId { get; set; }
    }
}
