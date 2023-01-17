using FairPlaySocial.Models.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace FairPlaySocial.Notifications.Hubs.Post
{
    public class PostNotificationHub : Hub<IPostNotificationHub>
    {
        public async Task SendMessage(PostNotificationModel model)
        {
            await Clients.Group(model.GroupName).ReceiveMessage(model);
        }

        public Task SendMessageToCaller(PostNotificationModel model)
        {
            return Clients.Caller.ReceiveMessage(model);
        }

        public Task JoinGroup(string groupName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string groupName)
        {
            return Groups
                .RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
