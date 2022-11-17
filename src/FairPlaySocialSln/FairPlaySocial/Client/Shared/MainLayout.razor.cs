using FairPlaySocial.Common;
using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FairPlaySocial.Client.Shared
{
    public partial class MainLayout
    {
        [Inject]
        private INavigationService? NavigationService { get; set; }
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        private void OnLogoutClickedAsync()
        {
            UserState.UserContext = new();
            NavigationManager!.NavigateToLogout("authentication/logout");
        }
        private void OnLoginClicked()
        {
            this.NavigationManager!.NavigateTo("authentication/login");
        }
    }
}
