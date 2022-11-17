namespace FairPlaySocial.Common.Interfaces.Services
{
    public interface INavigationService
    {
        string CurrentUrl { get; }
        void NavigateHome(bool forceReload);
        void NavigateBack();
        void NavigateToUserListForAdmin();
        void NavigateToUpdateMyUserPreferences();
    }
}
