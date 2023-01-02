using FairPlaySocial.Common;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.MAUIBlazor.Features.LogOn;
using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Client;

namespace FairPlaySocial.MAUIBlazor.Shared
{
    public partial class MainLayout
    {
        [Inject]
        private B2CConstants? B2CConstants { get; set; }
        [Inject]
        private INavigationService? NavigationService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        [Inject]
        private ICultureSelectionService? CultureSelectionService { get; set; }
        private bool ShowCultureSelector { get; set; }

        private async Task OnLogoutClickedAsync()
        {
            //Based on https://learn.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/authentication/azure-ad-b2c
            var accounts = await B2CConstants!.PublicClientApp!.GetAccountsAsync();
            while (accounts.Any())
            {
                await B2CConstants.PublicClientApp.RemoveAsync(accounts.First());
                accounts = await B2CConstants.PublicClientApp.GetAccountsAsync();
                UserState.UserContext = new();
                this.NavigationService!.NavigateHome(forceReload: true);
            }
        }
        private async Task OnLoginClickedAsync()
        {
            AuthenticationResult? authResult = null;
            IEnumerable<IAccount> accounts = await B2CConstants!.PublicClientApp!.GetAccountsAsync();
            try
            {
                IAccount? currentUserAccount = 
                    MainLayout.GetAccountByPolicy(accounts, 
                    B2CConstants!.PolicySignUpSignIn!);
                authResult = await B2CConstants.PublicClientApp
                    .AcquireTokenSilent(B2CConstants.ApiScopesArray, currentUserAccount)
                    .ExecuteAsync();
                CompleteAuthentication(authResult);
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    var currentCultureInfo = this.CultureSelectionService!
                        .GetCurrentCulture();
                    authResult = await B2CConstants.PublicClientApp
                        .AcquireTokenInteractive(B2CConstants.ApiScopesArray)
#if ANDROID
                        .WithParentActivityOrWindow(Platform.CurrentActivity)
#endif
                        .WithAccount(
                        MainLayout.GetAccountByPolicy(accounts, 
                        B2CConstants!.PolicySignUpSignIn!))
                        .WithExtraQueryParameters($"ui_locales={currentCultureInfo.Name}")
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                    CompleteAuthentication(authResult);
                }
                catch (Exception ex)
                {
                    await this.ToastService!.ShowErrorMessageAsync(ex.Message,
                        CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                string message = $"Users:{string.Join(",", accounts.Select(u => u.Username))}{Environment.NewLine}Error Acquiring Token:{Environment.NewLine}{ex}";
                await this.ToastService!.ShowErrorMessageAsync(message, CancellationToken.None);
            }
        }

        private void CompleteAuthentication(AuthenticationResult authResult)
        {
            UserState.UserContext = new UserContext()
            {
                AccessToken = authResult.AccessToken,
                IsLoggedOn = true,
                UserIdentifier = authResult.UniqueId,
                FullName = authResult.ClaimsPrincipal.Claims.Single(p => p.Type == Constants.Claims.Name).Value
            };
            NavigationService!.NavigateHome(forceReload: true);
        }

        private static IAccount? GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
        {
            foreach (var account in accounts)
            {
                string userIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
                if (userIdentifier.EndsWith(policy.ToLower())) return account;
            }

            return null;
        }

        private void OnShowCultureSelectorClicked()
        {
            this.ShowCultureSelector = true;
        }

        private void HideCultureSelector()
        {
            this.ShowCultureSelector = false;
        }
    }
}
