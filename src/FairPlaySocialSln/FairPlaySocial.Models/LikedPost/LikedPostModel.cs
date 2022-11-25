using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.LikedPost
{
    public class LikedPostModel
    {
        [Required]
        public long? LikedPostId { get; set; }
        [Required]
        public long? PostId { get; set; }
        [Required]
        public long? LikingApplicationUserId { get; set; }
    }
}
