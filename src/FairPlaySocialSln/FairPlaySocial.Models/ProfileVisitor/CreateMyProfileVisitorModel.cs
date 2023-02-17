using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.ProfileVisitor
{
    /// <summary>
    /// Represent a visit from the logged in user to the specified <see cref="VisitedApplicationUserId"/>
    /// </summary>
    public class CreateMyProfileVisitorModel
    {

        [Required]
        public long? VisitedApplicationUserId { get; set; }
    }
}
