using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.UserProfile
{
    public class CreateUserProfileModel
    {
        [Required]
        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(50)]
        public string? LinkedInNickname { get; set; }
        public string? LinkedInLink =>
            String.IsNullOrWhiteSpace(LinkedInNickname) ?
            String.Empty :
            $"https://www.linkedin.com/in/{LinkedInNickname}";

        [StringLength(50)]
        public string? TwitterNickname { get; set; }

        [StringLength(50)]
        public string? FacebookNickname { get; set; }

        [StringLength(50)]
        public string? InstagramNickname { get; set; }
        [StringLength(50)]
        public string? BuyMeACoffeeNickname { get; set; }
        [StringLength(50)]
        public string? GithubSponsorsNickname { get; set; }

        [StringLength(50)]
        public string? YouTubeNickname { get; set; }
        public string? YouTubeLink =>
            String.IsNullOrWhiteSpace(YouTubeNickname) ?
            String.Empty :
            $"https://youtube.com/c/{YouTubeNickname}";
    }
}
