using FairPlaySocial.Common.Global;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using static FairPlaySocial.Common.Global.Constants;

namespace FairPlaySocial.SharedUI.Pages.User.UserProfile
{
    [Authorize(Roles = $"{Constants.Roles.User}")]
    [Route(Constants.MauiBlazorAppPages.UserRolePagesRoutes.UpdateMyUserProfile)]
    public partial class UpdateMyUserProfile
    {
    }
}
