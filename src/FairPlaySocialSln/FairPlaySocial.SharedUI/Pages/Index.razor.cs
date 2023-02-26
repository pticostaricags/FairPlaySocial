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

        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.IsLoading = true;
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
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
                                AddAdminRoleMenuItems();
                            }
                            else if (state.User.IsInRole(Constants.Roles.User))
                            {
                                AddUserRoleMenuItems();
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
        }

        private void AddAdminRoleMenuItems()
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
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-translate",
                                    OnClick= new EventCallback(this, ()=>this.NavigationService!
                                    .NavigateToSupportedCultures()),
                                    ShowTitleBelowIcon=true,
                                    Title=Localizer![SupportedCulturesTextKey]
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-translate",
                                    OnClick= new EventCallback(this, ()=>this.NavigationService!
                                    .NavigateToResourceKeysAdmin()),
                                    ShowTitleBelowIcon=true,
                                    Title=Localizer![ResourceKeysAdminTextKey]
                                }
            };
        }

        private void AddUserRoleMenuItems()
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
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-folder-plus",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToCreateMyGroup()),
                                    ShowTitleBelowIcon=true,
                                    Title=Localizer![CreateMyGroupTextKey]
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-folder-plus",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToUserMessages()),
                                    ShowTitleBelowIcon=true,
                                    Title=Localizer![UserMessagesTextKey]
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-eye-fill",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToMyProfileVisitors()),
                                    ShowTitleBelowIcon=true,
                                    Title=Localizer![MyProfileVisitorsTextKey]
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-eye-fill",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToExternalReportsViewer()),
                                    ShowTitleBelowIcon=true,
                                    Title=Localizer![ExternalReportsTextKey]
                                }
            };
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
        [ResourceKey(defaultValue: "Create My Group")]
        public const string CreateMyGroupTextKey = "CreateMyGroupText";
        [ResourceKey(defaultValue: "User Messages")]
        public const string UserMessagesTextKey = "UserMessagesText";
        [ResourceKey(defaultValue: "My Profile Visitors")]
        public const string MyProfileVisitorsTextKey = "MyProfileVisitorsText";
        [ResourceKey(defaultValue: "Exteral Reports")]
        public const string ExternalReportsTextKey = "ExternalReportsText";
        [ResourceKey(defaultValue: "Supported Cultures")]
        public const string SupportedCulturesTextKey = "SupportedCulturesText";
        [ResourceKey(defaultValue: "Resource Keys Admin")]
        public const string ResourceKeysAdminTextKey = "ResourceKeysAdminText";
        #endregion Resource Keys
    }
}