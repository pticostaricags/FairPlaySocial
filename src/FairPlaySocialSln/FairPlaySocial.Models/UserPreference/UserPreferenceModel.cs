using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.UserPreference
{
    public class UserPreferenceModel
    {
        public long? UserPreferenceId { get; set; }
        public long? ApplicationUserId { get; set; }
        [Required]
        public bool EnableAudibleCuesInMobile { get; set; }
        [Required]
        public bool EnableAudibleCuesInWeb { get; set; }
    }
}
