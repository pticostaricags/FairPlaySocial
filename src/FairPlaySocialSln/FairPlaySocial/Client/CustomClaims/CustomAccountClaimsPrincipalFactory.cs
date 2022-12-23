using FairPlaySocial.ClientServices;
using FairPlaySocial.Common;
using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Security.Claims;

namespace FairPlaySocial.Client.CustomClaims
{
    public class CustomAccountClaimsPrincipalFactory : AccountClaimsPrincipalFactory<CustomRemoteUserAccount>
    {
        private readonly ApplicationUserClientService ApplicationUserClientService;
        private readonly IToastService ToastService;
        private IAccessTokenProviderAccessor AccessTokenProviderAccessor { get; }

        public CustomAccountClaimsPrincipalFactory(
            IAccessTokenProviderAccessor accessTokenProviderAccessor,
            ApplicationUserClientService applicationUserClientService,
            IToastService toastService) : base(accessTokenProviderAccessor)
        {
            this.ApplicationUserClientService = applicationUserClientService;
            this.AccessTokenProviderAccessor = accessTokenProviderAccessor;
            this.ToastService = toastService;
        }

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(CustomRemoteUserAccount account,
            RemoteAuthenticationUserOptions options)
        {
            var userClaimsPrincipal = await base.CreateUserAsync(account, options);
            if (userClaimsPrincipal!.Identity!.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = (userClaimsPrincipal!.Identity as ClaimsIdentity)!;
                try
                {
                    var userRoles = await ApplicationUserClientService.GetMyRolesAsync(CancellationToken.None);
                    if (userRoles != null)
                        foreach (var singleRole in userRoles)
                        {
                            claimsIdentity.AddClaim(new Claim("Role", singleRole));
                        }
                    var accessToken = await this.AccessTokenProviderAccessor.TokenProvider.RequestAccessToken();
                    accessToken.TryGetToken(out var token);
                    UserState.UserContext = new UserContext()
                    {
                        AccessToken = token!.Value,
                        IsLoggedOn = true,
                        UserIdentifier = claimsIdentity!.FindFirst("oid")!.Value,
                        FullName = claimsIdentity!.FindFirst("name")!.Value
                    };
                }
                catch (Exception ex)
                {
                    await this.ToastService!.ShowErrorMessageAsync(ex.Message, CancellationToken.None);
                }
            }
            return userClaimsPrincipal;
        }
    }
}
