using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.Pagination
{
    /// <summary>
    /// Handles the paging page request
    /// </summary>
    public class PageRequestModel
    {
        /// <summary>
        /// Page number beinb requested
        /// </summary>
        [Required]
        public int? PageNumber { get; set; }
    }
}
