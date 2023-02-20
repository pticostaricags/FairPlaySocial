using Microsoft.Identity.Client;

namespace FairPlaySocial.MAUIBlazor.Features.LogOn
{
    public class B2CConstants
    {
        public string? Tenant { get; set; }
        public string? AzureADB2CHostname { get; set; }
        public string? ClientId { get; set; }
        public string? PolicySignUpSignIn { get; set; }
        public string? PolicyEditProfile { get; set; }
        public string? PolicyResetPassword { get; set; }

        public string? ApiScopes { get; set; }
        public string[]? ApiScopesArray => ApiScopes?.Split(",");
        public string? Authority { get; set; }

        public string? AuthorityBase => $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
        public string? AuthorityEditProfile => $"{AuthorityBase}{PolicyEditProfile}";
        public string? AuthorityResetPassword { get; set; }
        public string? RedirectUri { get; set; }
        public IPublicClientApplication? PublicClientApp { get; set; }
    }
}
