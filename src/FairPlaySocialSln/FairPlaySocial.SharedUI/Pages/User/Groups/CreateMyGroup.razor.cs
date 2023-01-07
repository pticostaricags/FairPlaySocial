using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.SharedUI.Pages.User.Groups
{
    [Authorize(Roles = Constants.Roles.User)]
    [Route(Constants.MauiBlazorAppPages.UserRolePagesRoutes.CreateMyGroup)]
    public partial class CreateMyGroup
    {
        [Inject]
        private MyGroupClientService? MyGroupClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        [Inject]
        private INavigationService? NavigationService { get; set; }
        private CreateGroupModel createGroupModel = new();
        private bool IsBusy { get; set; }

        private async Task OnValidSubmitAsync()
        {
            try
            {
                IsBusy = true;
                await this.MyGroupClientService!
                    .CreateMyGroupAsync(this.createGroupModel, base.CancellationToken);
                this.NavigationService!.NavigateHome(false);
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
