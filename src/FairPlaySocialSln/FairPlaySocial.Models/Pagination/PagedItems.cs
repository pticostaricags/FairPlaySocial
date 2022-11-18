using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.Pagination
{

    /// <summary>
    /// Handles the returned paged items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedItems<T>
    {
        /// <summary>
        /// Page Number
        /// </summary>
        [Required]
        public int? PageNumber { get; set; }
        /// <summary>
        /// Total Items
        /// </summary>
        [Required]
        public int? TotalItems { get; set; }
        /// <summary>
        /// Page Size
        /// </summary>
        [Required]
        public int? PageSize { get; set; }
        /// <summary>
        /// Items for page
        /// </summary>
        [Required]
        public T[]? Items { get; set; }
        /// <summary>
        /// Total Pages
        /// </summary>
        [Required]
        public int? TotalPages { get; set; }
    }
}
