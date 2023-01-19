using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.SharedUI.Pages.User.Feeds
{
    [Authorize(Roles = $"{Constants.Roles.User}")]
    [Route($"{Constants.MauiBlazorAppPages.UserRolePagesRoutes.GroupFeed}/{{groupId:long}}")]
    public partial class GroupFeed
    {
        [Parameter]
        public long? GroupId { get; set; }
        [Inject]
        private MyFeedClientService? MyFeedClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        [Inject]
        private IAnalyticsService? AppCenterService { get; set; }
        [Inject]
        private INavigationService? NavigationService { get; set; }
        private PageRequestModel PageRequestModel { get; set; } = new PageRequestModel()
        {
            PageNumber = 1
        };
        private List<PostModel>? PostModels { get; set; }
        private bool IsBusy { get; set; }

        private PagedItems<PostModel>? MyHomeFeed;

        protected override async Task OnInitializedAsync()
        {
            this.AppCenterService?.LogEvent(EventType.LoadGroupFeed);
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsBusy = true;
                this.MyHomeFeed = null;
                StateHasChanged();
                this.MyHomeFeed = await this.MyFeedClientService!
                    .GetGroupFeedAsync(this.PageRequestModel, this.GroupId, base.CancellationToken);
                this.PostModels = MyHomeFeed!.Items!.ToList();
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

        private void OnNavigateToCreatePostClicked()
        {
            this.NavigationService!.NavigateToCreateMyPostInGroup(this.GroupId!.Value);
        }
    }
}
