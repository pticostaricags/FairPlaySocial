using FairPlaySocial.ClientServices;
using FairPlaySocial.Common;
using FairPlaySocial.Common.Extensions;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.VisitorTracking;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Timers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FairPlaySocial.Client.Shared
{
    public partial class MainLayout
    {
        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }
        [CascadingParameter]
        private Error? Error { get; set; }
        [Inject]
        private INavigationService? NavigationService { get; set; }
        [Inject]
        private NavigationManager? NavigationManager { get; set; }
        [Inject]
        private VisitorTrackingClientService? VisitorTrackingClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private bool ShowCultureSelector { get; set; }
        private CancellationTokenSource CancellationTokenSource = new();
        private System.Timers.Timer? VisitsTimer { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await TrackVisitAsync(createNewSession: true);
                this.NavigationManager!.LocationChanged += NavigationManager_LocationChanged;
            }
            catch (Exception ex)
            {
                await this.Error!.ProcessErrorAsync(ex, this.CancellationTokenSource.Token);
                await ToastService!
                    .ShowErrorMessageAsync(ex.Message, this.CancellationTokenSource.Token);
            }
        }

        private async Task TrackVisitAsync(bool createNewSession)
        {
            //We do not want to track authentication flow pages visits
            if (this.NavigationManager!.Uri.Contains("/authentication/"))
                return;
            VisitorTrackingModel visitorTrackingModel = new()
            {
                VisitedUrl = this.NavigationManager.Uri
            };
            var state = await AuthenticationStateTask!;
            if (state != null && state.User != null && state.User.Identity!.IsAuthenticated)
            {
                var userObjectId = state.User.Claims.GetAzureAdB2CUserObjectId();
                visitorTrackingModel.UserAzureAdB2cObjectId = userObjectId;
                await this.VisitorTrackingClientService!.
                    TrackAuthenticatedVisitAsync(visitorTrackingModel,
                    createNewSession,
                    this.CancellationTokenSource.Token);
            }
            else
            {
                await this.VisitorTrackingClientService!
                    .TrackAnonymousVisitAsync(visitorTrackingModel,
                    createNewSession,
                    this.CancellationTokenSource.Token);
            }

            if (createNewSession)
            {
                this.VisitsTimer = new (TimeSpan.FromSeconds(60).TotalMilliseconds);
                this.VisitsTimer.Elapsed += VisitsTimer_Elapsed;
                this.VisitsTimer.Start();
            }
        }

        private async void VisitsTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                await VisitorTrackingClientService!.UpdateVisitTimeElapsedAsync(this.CancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                await this.Error!.ProcessErrorAsync(ex, this.CancellationTokenSource.Token);
            }
        }

        private async void NavigationManager_LocationChanged(object? sender, 
            Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            try
            {
                await TrackVisitAsync(createNewSession: false);
            }
            catch (Exception ex)
            {
                await this.Error!.ProcessErrorAsync(ex, this.CancellationTokenSource.Token);
            }
        }

        private void OnLogoutClickedAsync()
        {
            UserState.UserContext = new();
            NavigationManager!.NavigateToLogout("authentication/logout");
        }
        private void OnLoginClicked()
        {
            this.NavigationManager!.NavigateTo("authentication/login");
        }

        private void OnShowCultureSelectorClicked()
        {
            this.ShowCultureSelector = true;
        }

        private void HideCultureSelector()
        {
            this.ShowCultureSelector = false;
        }
    }
}
