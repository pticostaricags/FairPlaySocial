using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.CustomAttributes.Localization;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Group;
using FairPlaySocial.Models.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Runtime.Versioning;

namespace FairPlaySocial.SharedUI.Pages.User.Search
{
    [Authorize(Roles = Constants.Roles.User)]
    [Route($"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.SearchGroups}")]
    public partial class SearchGroups
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public string? SearchTerm { get; set; }
        [Inject]
        private SearchClientService? SearchClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }

        [Inject]
        private MyGroupClientService? MyGroupClientService { get; set; }
        private bool IsBusy { get; set; }
        private PagedItems<GroupModel>? Groups { get; set; }

        private PageRequestModel PageRequestModel = new PageRequestModel()
        {
            PageNumber = 1
        };

        protected async override Task OnParametersSetAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                this.IsBusy = true;
                this.Groups = await this.SearchClientService!
                    .SearchGroupsAsync(this.PageRequestModel, this.SearchTerm!, base.CancellationToken);
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

        private async Task OnJoinGroupClickedAsync(long groupId)
        {
            try
            {
                this.IsBusy = true;
                await this.MyGroupClientService!.JoinGroupAsync(groupId, base.CancellationToken);
                await ToastService!.ShowSuccessMessageAsync("You have joined the group", base.CancellationToken);
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
