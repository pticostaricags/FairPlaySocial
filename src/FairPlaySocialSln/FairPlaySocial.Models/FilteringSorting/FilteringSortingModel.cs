using FairPlaySocial.Models.Filtering;
using FairPlaySocial.Models.Sorting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.FilteringSorting
{
    public class FilteringSortingModel
    {
        [Required]
        [ValidateComplexType]
        public FilteringModel? Filtering { get; set; }
        [Required]
        [ValidateComplexType]
        public SortingModel[]? Sorting { get; set; }
    }
}
