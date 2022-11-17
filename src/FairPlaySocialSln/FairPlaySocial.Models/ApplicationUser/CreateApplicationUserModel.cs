using System.ComponentModel.DataAnnotations;

namespace FairPlaySocial.Models.ApplicationUser
{
    public class CreateApplicationUserModel
    {
        [Required]
        [StringLength(150)]
        public string? FullName { get; set; }
        [Required]
        [StringLength(150)]
        public string? EmailAddress { get; set; }
        public DateTimeOffset LastLogIn { get; set; }
        public Guid AzureAdB2cobjectId { get; set; }
    }
}
