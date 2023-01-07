using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.Group
{
    public class CreateGroupModel
    {
        [Required]
        public long? OwnerApplicationUserId { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required]
        [StringLength(250)]
        public string? Description { get; set; }

        [Required]
        [StringLength(100)]
        public string? TopicTag { get; set; }
    }
}
