namespace FairPlaySocial.Common.Interfaces.Services
{
    public interface INavigationService
    {
        string CurrentUrl { get; }
        void NavigateHome(bool forceReload);
        void NavigateBack();
        void NavigateToUserListForAdmin();
        void NavigateToUpdateMyUserPreferences();
        void NavigateToCreateMyPost();
        void NavigateToCreateMyPostInGroup(long groupId);
        void NavigateToHomeFeed();
        void NavigateToUpdateMyUserProfile();
        void NavigateToPost(long postId);
        string GetAbsoluteUrl(string relativePath);
        void NavigateToCreateMyGroup();
        void NavigateToSearchUserProfiles(string searchTerm);
        void NavigateToSearchPosts(string searchTerm);
        void NavigateToSearchGroups(string searchTerm);
        void NavigateToGroupFeed(long groupId);
        void NavigateToUserMessages();
        void NavigateToMyProfileVisitors();
        void NavigateToExternalReportsViewer();
        void NavigateToSupportedCultures();
        void NavigateToResourceKeysAdmin();
    }
}
