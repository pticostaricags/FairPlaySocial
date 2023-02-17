using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.ProfileVisitor
{
    public class ProfileVisitorModel
    {
        public long VisitorApplicationUserId { get; set; }

        public long VisitedApplicationUserId { get; set; }

        public DateTimeOffset RowCreationDateTime { get; set; }
        public string? VisitorFullName { get; set; }
        public string? VisitorBio { get; set; }
    }
}
