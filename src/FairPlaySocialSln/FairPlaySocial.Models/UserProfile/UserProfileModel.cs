using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.UserProfile
{
    public class UserProfileModel
    {
        [Required]
        public long? UserProfileId { get; set; }
        [Required]
        public long? ApplicationUserId { get; set; }

        [Required]
        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(50)]
        public string? LinkedInNickname { get; set; }

        [StringLength(50)]
        public string? TwitterNickname { get; set; }

        [StringLength(50)]
        public string? FacebookNickname { get; set; }

        [StringLength(50)]
        public string? InstagramNickname { get; set; }

        [StringLength(50)]
        public string? YouTubeNickname { get; set; }
    }
}
