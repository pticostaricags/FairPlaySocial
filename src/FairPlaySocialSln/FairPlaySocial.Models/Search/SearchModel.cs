using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.Search
{
    public class SearchModel
    {
        [Required]
        public string? SearchTerm { get; set; }
        [Required]
        public SearchType? SearchType { get; set; }
    }

    public enum SearchType
    {
        UserProfiles,
        Posts,
        Groups
    }
}
