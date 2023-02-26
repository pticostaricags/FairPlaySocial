using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.Culture
{
    public class CultureModel
    {
        [Key]
        public int? CultureId { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
    }
}
