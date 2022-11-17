using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.Sorting
{
    public class SortingModel
    {
        [Required]
        public string? ColumnName { get; set; }
        public string? Sort { get; set; }

        public string PairAsSqlExpression
        {
            get
            {
                return $"{ColumnName} {Sort}";
            }
        }
    }
}
