using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientsConfiguration
{
    public class AppSettings
    {
        public Azureadb2c? AzureAdB2C { get; set; }
    }

    public class Azureadb2c
    {
        public string? Authority { get; set; }
        public string? ClientId { get; set; }
        public bool ValidateAuthority { get; set; }
        //public string? ResetPasswordPolicyId { get; set; }
        //public string? ResetPasswordCallbackUrl { get; set; }
        public string? ProfileEditPolicyId { get; set; }
        public string? ProfileEditCallbackUrl { get; set; }
    }
}
