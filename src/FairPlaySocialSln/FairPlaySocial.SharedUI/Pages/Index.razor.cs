using FairPlaySocial.ClientServices;
using FairPlaySocial.Common;
using FairPlaySocial.Common.CustomAttributes.Localization;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.MultiplatformComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;

namespace FairPlaySocial.SharedUI.Pages
{
    public partial class Index
    {
        [Inject]
        private INavigationService? NavigationService { get; set; }
        [Inject]
        private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        [Inject]
        private IStringLocalizer<Index>? Localizer { get; set; }
        [Inject]
        private LocalizationClientService? LocalizationClientService { get; set; }
        [Inject]
        private IAnalyticsService? AppCenterService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private MenuGrid.MenuGridItem[]? MainMenuItems { get; set; }
        private bool IsLoading { get; set; } = false;
        private string? WelcomeMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.AppCenterService?.LogEvent(EventType.LoadIndexPage);
                IsLoading = true;
                await base.OnInitializedAsync();
                await this.LocalizationClientService!.LoadDataAsync();
                this.WelcomeMessage = String.Format(
                    Localizer![WelcomeMessageTextKey],
                    base.WhiteLabelingService!.WhiteLabelingData!.ApplicationName);
                if (UserState.UserContext?.IsLoggedOn == true)
                {
                    var state = await this.AuthenticationStateProvider!.GetAuthenticationStateAsync();
                    if (state != null)
                    {
                        if (state.User.IsInRole(Constants.Roles.Admin))
                        {
                            this.MainMenuItems = new MenuGrid.MenuGridItem[]
                            {
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-people-fill",
                                    OnClick= new EventCallback(this, ()=>this.NavigationService!
                                    .NavigateToUserListForAdmin()),
                                    ShowTitleBelowIcon=true,
                                    Title=Localizer![UserListTextKey]
                                }
                            };
                        }
                        else if (state.User.IsInRole(Constants.Roles.User))
                        {
                            this.MainMenuItems = new MenuGrid.MenuGridItem[]
                            {
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-gear-fill",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToUpdateMyUserPreferences()),
                                    ShowTitleBelowIcon=true,
                                    Title=Localizer![UserPreferencesTextKey]
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-person-lines-fill",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToUpdateMyUserProfile()),
                                    ShowTitleBelowIcon=true,
                                    Title=Localizer![UserProfileTextKey]
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-pencil-square",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToCreateMyPost()),
                                    ShowTitleBelowIcon=true,
                                    Title=Localizer![CreatePostTextKey]
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-collection-fill",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToHomeFeed()),
                                    ShowTitleBelowIcon=true,
                                    Title=Localizer![HomeFeedTextKey]
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await ToastService!.ShowErrorMessageAsync(ex.Message, base.CancellationToken);
                this.AppCenterService?.LogException(ex);
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "Welcome to {0}. The Multi-platform system to share your thoughts.")]
        public const string WelcomeMessageTextKey = "WelcomeMessageText";
        [ResourceKey(defaultValue: "User List")]
        public const string UserListTextKey = "UserListText";
        [ResourceKey(defaultValue: "User Preferences")]
        public const string UserPreferencesTextKey = "UserPreferencesText";
        [ResourceKey(defaultValue: "User Profile")]
        public const string UserProfileTextKey = "UserProfileText";
        [ResourceKey(defaultValue: "Create Post")]
        public const string CreatePostTextKey = "CreatePostText";
        [ResourceKey(defaultValue: "Home Feed")]
        public const string HomeFeedTextKey = "HomeFeedText";
        #endregion Resource Keys
    }
}