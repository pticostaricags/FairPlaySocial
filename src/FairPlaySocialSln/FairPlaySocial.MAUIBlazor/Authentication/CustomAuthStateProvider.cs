using FairPlaySocial.ClientServices;
using FairPlaySocial.Common;
using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FairPlaySocial.MAUIBlazor.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ApplicationUserClientService UserClientService;

        private readonly IToastService ToastService;

        public CustomAuthStateProvider(ApplicationUserClientService userClientService,
            IToastService toastService)
        {
            this.UserClientService = userClientService;
            this.ToastService = toastService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                ClaimsIdentity identity = new();
                if (UserState.UserContext.IsLoggedOn)
                {
                    var roles = await this.UserClientService.GetMyRolesAsync(
                        CancellationToken.None);
                    if (roles != null)
                    {
                        foreach (var singleUserRole in roles)
                        {
                            identity.AddClaim(new Claim(type: ClaimTypes.Role, value: singleUserRole));
                        }
                    }
                    identity.AddClaim(new Claim("oid", UserState.UserContext!.UserIdentifier!));
                }
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
            catch (Exception ex)
            {
                await this.ToastService.ShowErrorMessageAsync(ex.Message, CancellationToken.None);
                throw;
            }
        }
    }
}
