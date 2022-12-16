using FairPlaySocial.Models.Photo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.Post
{
    public class PostModel
    {
        [Required]
        public long? PostId { get; set; }

        [Required]
        [StringLength(500)]
        public string? Text { get; set; }
        public DateTimeOffset? RowCreationDateTime { get; set; }
        public string? OwnerApplicationUserFullName { get; set; }
        public long? OwnerApplicationUserId { get; set; }
        public PhotoModel? Photo { get; set; }
        public TimeSpan PostedTime =>
            DateTimeOffset.UtcNow.Subtract(RowCreationDateTime!.Value);
        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
        public int LikesCount { get; set; }
        public string? Tag1 { get; set; }
        public string? Tag2 { get; set; }
        public string? Tag3 { get; set; }
        [Url]
        public string? Url { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsOwned { get; set; }
    }
}
