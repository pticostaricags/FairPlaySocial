using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using static FairPlaySocial.Common.Global.Constants;

namespace FairPlaySocial.SharedUI.Pages.User.UserProfile
{
    [Authorize(Roles = $"{Constants.Roles.User}")]
    [Route(Constants.MauiBlazorAppPages.UserRolePagesRoutes.UpdateMyUserProfile)]
    public partial class UpdateMyUserProfile
    {
        [Inject]
        private MyUserProfileClientService? MyUserProfileClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        [Inject]
        private INavigationService? NavigationService { get; set; }
        private CreateUserProfileModel createUserProfileModel = new();
        private bool IsBusy { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.IsBusy = true;
                var existenUserProfile = await this.MyUserProfileClientService!
                    .GetMyUserProfileAsync(base.CancellationToken);
                this.createUserProfileModel.Bio = existenUserProfile.Bio;
                this.createUserProfileModel.FacebookNickname = existenUserProfile.FacebookNickname;
                this.createUserProfileModel.YouTubeNickname = existenUserProfile.YouTubeNickname;
                this.createUserProfileModel.LinkedInNickname = existenUserProfile.LinkedInNickname;
                this.createUserProfileModel.InstagramNickname = existenUserProfile.InstagramNickname;
                this.createUserProfileModel.TwitterNickname = existenUserProfile.TwitterNickname;
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnValidSubmitAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.MyUserProfileClientService!
                    .UpdateMyUserProfileAsync(this.createUserProfileModel!,
                    base.CancellationToken);
                await this.ToastService!
                    .ShowSuccessMessageAsync(
                    "Your profile has been updated", base.CancellationToken);
                this.NavigationService!.NavigateHome(forceReload: false);
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}
