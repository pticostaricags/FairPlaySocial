using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FairPlaySocial.Models.ApplicationUser
{
    public class ApplicationUserModel
    {
        public long ApplicationUserId { get; set; }
        [Required]
        [StringLength(150)]
        public string? FullName { get; set; }
        [Required]
        [StringLength(150)]
        public string? EmailAddress { get; set; }
        public DateTimeOffset LastLogIn { get; set; }
        [Column("AzureAdB2CObjectId")]
        public Guid AzureAdB2cobjectId { get; set; }
    }
}
