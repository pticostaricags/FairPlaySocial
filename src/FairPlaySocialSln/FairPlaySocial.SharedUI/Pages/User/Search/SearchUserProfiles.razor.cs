using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.SharedUI.Pages.User.Search
{
    [Authorize(Roles = Constants.Roles.User)]
    [Route($"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.SearchUserProfiles}")]
    public partial class SearchUserProfiles
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public string? SearchTerm { get; set; }
        [Inject]
        private SearchClientService? SearchClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private bool IsBusy { get; set; }
        private PagedItems<UserProfileModel>? UserProfiles { get; set; }

        private PageRequestModel PageRequestModel = new PageRequestModel()
        {
            PageNumber = 1
        };
        private UserProfileModel? SelectedUserProfileModel { get; set; }
        private bool ShowComposeMessageModal { get; set; }

        protected async override Task OnParametersSetAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                this.IsBusy = true;
                this.UserProfiles = await this.SearchClientService!
                    .SearchUserProfilesAsync(this.PageRequestModel, this.SearchTerm!, base.CancellationToken);
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

        private void ShowComposeMessage(UserProfileModel userProfileModel)
        {
            this.SelectedUserProfileModel = userProfileModel;
            this.ShowComposeMessageModal = true;
        }

        private void HideComposeMessage()
        {
            this.ShowComposeMessageModal = false;
            this.SelectedUserProfileModel = null;
        }

        private async Task OnMessageSentAsync()
        {
            await this.ToastService!
                .ShowSuccessMessageAsync("Message has been sent", base.CancellationToken);
            this.HideComposeMessage();
        }
    }
}
