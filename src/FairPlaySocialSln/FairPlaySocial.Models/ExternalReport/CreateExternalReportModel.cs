using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.ExternalReport
{
    public class CreateExternalReportModel
    {
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required]
        public string? ExternalUrl { get; set; }
    }
}
