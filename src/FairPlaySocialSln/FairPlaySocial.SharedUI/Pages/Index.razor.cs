using FairPlaySocial.ClientServices;
using FairPlaySocial.Common;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.ApplicationUser;
using FairPlaySocial.MultiplatformComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FairPlaySocial.SharedUI.Pages
{
    public partial class Index
    {
        [Inject]
        private INavigationService? NavigationService { get; set; }
        [Inject]
        private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        private MenuGrid.MenuGridItem[]? MainMenuItems { get; set; }
        private bool IsLoading { get; set; } = false;
        private string? WelcomeMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                await base.OnInitializedAsync();
                this.WelcomeMessage = $"Welcome to " +
                    $"{base.WhiteLabelingService!.WhiteLabelingData!.ApplicationName}.\r\n" +
                    $"The Multi-platform system to share your thoughts.";
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
                                    CssClass="bi bi-building",
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
                                    CssClass="bi bi-building",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToUpdateMyUserPreferences()),
                                    ShowTitleBelowIcon=true,
                                    Title="User Preferences"
                                },
                                new MenuGrid.MenuGridItem()
                                {
                                    CssClass="bi bi-building",
                                    OnClick= new EventCallback(this, ()=>
                                    this.NavigationService!
                                    .NavigateToCreateMyPost()),
                                    ShowTitleBelowIcon=true,
                                    Title="Create Post"
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
    }
}