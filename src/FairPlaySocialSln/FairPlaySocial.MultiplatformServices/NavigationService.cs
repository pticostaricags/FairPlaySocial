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

        public void NavigateToCreateMyPostInGroup(long groupId)
        {
            this.NavigateTo(
                $"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.CreateMyPost}/{groupId}", false);
        }

        public void NavigateToHomeFeed()
        {
            this.NavigateTo(
                $"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.HomeFeed}", false);
        }

        public void NavigateToUpdateMyUserProfile()
        {
            this.NavigateTo(
                $"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.UpdateMyUserProfile}", false);
        }

        public void NavigateToPost(long postId)
        {
            this.NavigateTo(
                $"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.Post}/{postId}", false);
        }

        public string GetAbsoluteUrl(string relativePath)
        {
            return this.NavigationManager.ToAbsoluteUri(relativePath).ToString();
        }

        public void NavigateToCreateMyGroup()
        {
            this.NavigateTo(Constants.MauiBlazorAppPages.UserRolePagesRoutes.CreateMyGroup, false);
        }

        public void NavigateToSearchUserProfiles(string searchTerm)
        {
            this.NavigateTo($"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.SearchUserProfiles}" +
                $"?searchTerm={searchTerm}", false);
        }

        public void NavigateToSearchPosts(string searchTerm)
        {
            this.NavigateTo($"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.SearchPosts}" +
                $"?searchTerm={searchTerm}", false);
        }

        public void NavigateToSearchGroups(string searchTerm)
        {
            this.NavigateTo($"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.SearchGroups}" +
                $"?searchTerm={searchTerm}", false);
        }

        public void NavigateToGroupFeed(long groupId)
        {
            this.NavigateTo($"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.GroupFeed}/{groupId}",
                false);
        }

        public void NavigateToUserMessages()
        {
            this.NavigateTo($"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.UserMessages}", false);
        }

        public void NavigateToMyProfileVisitors()
        {
            this.NavigateTo($"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.MyProfileVisitors}", false);
        }

        public void NavigateToExternalReportsViewer()
        {
            this.NavigateTo($"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.ExternalReportsViewer}", false);
        }

        public void NavigateToSupportedCultures()
        {
            this.NavigateTo($"{Constants.MauiBlazorAppPages.AdminRolePagesRoutes.SupportedCultures}", false);
        }
    }
}
