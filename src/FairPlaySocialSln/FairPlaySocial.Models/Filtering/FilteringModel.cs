using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.Filtering
{
    public class FilteringModel
    {
        [Required]
        public string? PropertyName { get; set; }
        [Required]
        public string? PropertyComparisonValue { get; set; }
        [Required]
        public ComparisonOperator? ComparisonOperator { get; set; }
    }

    public enum ComparisonOperator
    {
        Equals=0,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith,
    }
}
