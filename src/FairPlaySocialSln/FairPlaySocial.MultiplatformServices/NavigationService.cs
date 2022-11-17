using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.MultiplatformServices
{
    public class NavigationService : INavigationService
    {
        private readonly NavigationManager NavigationManager;
        private static Stack<string> NavigationHistory { get; set; } = new();

        public string CurrentUrl => NavigationManager.ToBaseRelativePath(NavigationManager.Uri);

        public NavigationService(NavigationManager navigationManager)
        {
            NavigationManager = navigationManager;
        }

        private void NavigateTo(string url, bool forceReload)
        {
            if (url == "/")
            {
                NavigationHistory.Clear();
            }
            else
            {
                NavigationHistory.Push(CurrentUrl);
            }
            NavigationManager.NavigateTo(url, forceReload);
        }

        public void NavigateHome(bool forceReload)
        {
            NavigateTo("/", forceReload);
        }

        public void NavigateBack()
        {
            if (NavigationHistory.Count == 0)
                return;
            var url = NavigationHistory.Pop();
            if (url == "/")
                NavigationHistory.Clear();
            NavigationManager.NavigateTo(url);
        }

        public void NavigateToUserListForAdmin()
        {
            this.NavigateTo(
                $"{Constants.MauiBlazorAppPages.AdminRolePagesRoutes.UserList}", false);
        }

        public void NavigateToUpdateMyUserPreferences()
        {
            this.NavigateTo(
                $"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.UpdateMyUserPreferences}", false);
        }

        public void NavigateToCreateMyPost()
        {
            this.NavigateTo(
                $"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.CreateMyPost}", false);
        }
    }
}
