using FairPlaySocial.ClientServices;
using FairPlaySocial.Common;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Notifications;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.Post;
using FairPlaySocial.SharedUI.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace FairPlaySocial.SharedUI.Pages.User.Feeds
{
    [Authorize(Roles = $"{Constants.Roles.User}")]
    [Route(Constants.MauiBlazorAppPages.UserRolePagesRoutes.HomeFeed)]
    public partial class HomeFeed
    {
        [Inject]
        private HttpClientService? HttpClientService { get; set; }
        [Inject]
        private MyFeedClientService? MyFeedClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        [Inject]
        private IAppCenterService? AppCenterService { get; set; }
        private PageRequestModel PageRequestModel { get; set; } = new PageRequestModel()
        {
            PageNumber = 1
        };
        private List<PostModel>? PostModels { get; set; }
        private bool IsBusy { get; set; }

        private PagedItems<PostModel>? MyHomeFeed;

        protected override async Task OnInitializedAsync()
        {
            this.AppCenterService?.LogEvent(EventType.LoadHomeFeedPage);
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
                    .GetMyHomeFeedAsync(this.PageRequestModel, base.CancellationToken);
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
    }
}
