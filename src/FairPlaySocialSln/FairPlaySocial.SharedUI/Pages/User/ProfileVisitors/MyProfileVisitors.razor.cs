using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.ProfileVisitor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.SharedUI.Pages.User.ProfileVisitors
{
    [Authorize(Roles = Constants.Roles.User)]
    [Route($"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.MyProfileVisitors}")]
    public partial class MyProfileVisitors
    {
        [Inject]
        private ProfileVisitorClientService? ProfileVisitorClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private bool IsBusy { get; set; }
        private PagedItems<ProfileVisitorModel>? MyProfileVisitorsModel { get; set; }

        private PageRequestModel PageRequestModel = new PageRequestModel()
        {
            PageNumber = 1
        };
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                this.IsBusy = true;
                this.MyProfileVisitorsModel =
                    await this.ProfileVisitorClientService!
                    .GetMyProfileVisitorsAsync(this.PageRequestModel,
                    base.CancellationToken);
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally
            {
                this.IsBusy = false;
                StateHasChanged();
            }
        }

        private async Task OnPreviousPageButtonCllickedAsync()
        {
            this.PageRequestModel.PageNumber--;
            await LoadDataAsync();
        }
        private async Task OnNextPageButtonClickedAsync()
        {
            this.PageRequestModel.PageNumber++;
            await LoadDataAsync();
        }
    }
}
