using FairPlaySocial.ClientServices;
using FairPlaySocial.Common;
using FairPlaySocial.Common.CustomAttributes.Localization;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.ApplicationUser;
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
        private MenuGrid.MenuGridItem[]? MainMenuItems { get; set; }
        private bool IsLoading { get; set; } = false;
        private string? WelcomeMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
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
                                    Title="User List"
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
                                    Title="User Preferences"
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-person-lines-fill",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToUpdateMyUserProfile()),
                                    ShowTitleBelowIcon=true,
                                    Title="User Profile"
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-pencil-square",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToCreateMyPost()),
                                    ShowTitleBelowIcon=true,
                                    Title="Create Post"
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-collection-fill",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToHomeFeed()),
                                    ShowTitleBelowIcon=true,
                                    Title="Home Feed"
                                }
                            };
                        }
                    }
                }
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
        #endregion Resource Keys
    }
}