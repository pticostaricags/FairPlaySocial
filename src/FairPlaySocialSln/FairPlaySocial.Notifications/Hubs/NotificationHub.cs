using FairPlaySocial.Models.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace FairPlaySocial.Notifications.Hubs
{
    public class NotificationHub : Hub<IPostNotificationHub>
    {
        public async Task SendMessage(PostNotificationModel model)
        {
            await this.Clients.Group(model.GroupName).ReceiveMessage(model);
        }

        public Task SendMessageToCaller(PostNotificationModel model)
        {
            return Clients.Caller.ReceiveMessage(model);
        }

        public Task JoinGroup(string groupName)
        {
            return this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string groupName)
        {
            return this.Groups
                .RemoveFromGroupAsync(this.Context.ConnectionId, groupName);
        }
    }
}
