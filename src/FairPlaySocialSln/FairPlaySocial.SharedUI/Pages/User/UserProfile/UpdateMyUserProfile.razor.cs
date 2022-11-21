using FairPlaySocial.Common.Global;
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
        private CreateUserProfileModel createUserProfileModel = new();
        private bool IsBusy { get; set; }

        private Task OnValidSubmitAsync()
        {
            return Task.CompletedTask;
        }
    }
}
