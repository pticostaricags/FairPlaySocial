using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.Search;
using FairPlaySocial.Models.UserMessage;
using FairPlaySocial.Models.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.SharedUI.Pages.User.UserMessages
{
    [Authorize(Roles = Constants.Roles.User)]
    [Route($"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.UserMessages}")]
    public partial class UserMessages
    {
        [Inject]
        private SearchClientService? SearchClientService { get; set; }
        [Inject]
        private UserMessageClientService? UserMessageClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private bool IsBusy { get; set; } = false;
        private UserMessageModel[]? UserMessageModels { get; set; }
        private PagedItems<UserProfileModel>? FilteredUserProfiles { get; set; }
        private UserProfileModel? SelectedUserProfile { get; set; } = new();

        private SearchModel searchModel = new SearchModel()
        {
            SearchType = SearchType.UserProfiles
        };

        private async Task SearchUserAsync()
        {
            try
            {
                this.IsBusy = true;
                this.FilteredUserProfiles = await this.SearchClientService!
                .SearchUserProfilesAsync(new Models.Pagination.PageRequestModel()
                {
                    PageNumber = 1
                },
                this.searchModel!.SearchTerm!,
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
            }

            StateHasChanged();
        }

        private async Task OnSelectedUserProfileChangedAsync(long? selectedApplicationUserId)
        {
            try
            {
                this.IsBusy = true;
                await Task.Yield();
                this.SelectedUserProfile!.ApplicationUserId = null;
                this.UserMessageModels = null;
                StateHasChanged();
                this.SelectedUserProfile!.ApplicationUserId = selectedApplicationUserId;
                this.UserMessageModels = await this.UserMessageClientService!
                    .GetMyMessagesWithUserAsync(selectedApplicationUserId!.Value, base.CancellationToken);
                StateHasChanged();
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

        private async Task OnMessageSentAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.ToastService!
                    .ShowSuccessMessageAsync("Message has been sent", base.CancellationToken);
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
