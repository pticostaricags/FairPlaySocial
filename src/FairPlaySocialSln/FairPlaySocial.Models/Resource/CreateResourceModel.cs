using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.Resource
{
    public class CreateResourceModel
    {
        [Required]
        [StringLength(1500)]
        public string? Type { get; set; }

        [Required]
        [StringLength(50)]
        public string? Key { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string? Value { get; set; }

        [Required]
        public int? CultureId { get; set; }
    }
}
