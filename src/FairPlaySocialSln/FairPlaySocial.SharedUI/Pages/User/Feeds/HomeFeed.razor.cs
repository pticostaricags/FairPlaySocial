using FairPlaySocial.ClientServices;
using FairPlaySocial.Common;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Notifications;
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
        private List<NotificationModel> ReceivedNotifications { get; set; } = new List<NotificationModel>();
        private bool IsBusy { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsBusy = true;
                var myHomeFeed = await this.MyFeedClientService!
                    .GetMyHomeFeedAsync(base.CancellationToken);
                if (myHomeFeed != null)
                {
                    this.ReceivedNotifications.AddRange(myHomeFeed
                        .Select(p => new NotificationModel()
                        {
                            From = p.OwnerApplicationUserFullName,
                            Message = p.Text
                        }));
                }
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
