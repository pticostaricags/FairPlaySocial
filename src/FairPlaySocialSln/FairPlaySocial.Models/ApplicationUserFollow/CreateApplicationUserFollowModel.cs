using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.ApplicationUserFollow
{
    public class CreateApplicationUserFollowModel
    {
        [Required]

        public long? FollowerApplicationUserId { get; set; }

        [Required]
        public long? FollowedApplicationUserId { get; set; }
    }
}
