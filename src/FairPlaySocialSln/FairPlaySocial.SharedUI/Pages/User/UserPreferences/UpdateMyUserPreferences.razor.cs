using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.UserPreference;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.SharedUI.Pages.User.UserPreferences
{
    [Authorize(Roles = $"{Constants.Roles.User}")]
    [Route(Constants.MauiBlazorAppPages.UserRolePagesRoutes.UpdateMyUserPreferences)]
    public partial class UpdateMyUserPreferences
    {
        [Inject]
        private MyUserPreferenceClientService? MyUserPreferenceClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        [Inject]
        private INavigationService? NavigationService { get; set; }
        [Inject]
        private UserPreferenceModel? MyUserPreferences { get; set; }
        [Inject]
        private IAnalyticsService? AnalyticsService { get; set; }
        private bool IsLoading { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.AnalyticsService!.LogEvent(EventType.LoadUpdateUserPreferencesPage);
                var tmpUserPreferences = await this.MyUserPreferenceClientService!
                    .GetMyUserPreferencesAsync(base.CancellationToken);
                this.MyUserPreferences!.EnableAudibleCuesInMobile = tmpUserPreferences!.EnableAudibleCuesInMobile;
                this.MyUserPreferences!.EnableAudibleCuesInWeb = tmpUserPreferences!.EnableAudibleCuesInWeb;
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally
            {
                IsLoading = !IsLoading;
            }
        }

        private async Task OnValidSubmitAsync()
        {
            try
            {
                IsLoading = true;
                await this.MyUserPreferenceClientService!
                    .UpdateMyUserPreferencesAsync(this.MyUserPreferences!, base.CancellationToken);
                this.NavigationService!.NavigateHome(forceReload: false);
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
