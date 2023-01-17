using FairPlaySocial.Models.Notifications;
using FairPlaySocial.Models.UserMessage;
using Microsoft.AspNetCore.SignalR;

namespace FairPlaySocial.Notifications.Hubs.UserMessage
{
    public class UserMessageNotificationHub : Hub<IUserMessageNotificationHub>
    {
        public async Task SendMessage(UserMessageNotificationModel model)
        {
            await Clients.Group(model.GroupName).ReceiveMessage(model);
        }

        public Task SendMessageToCaller(UserMessageNotificationModel model)
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
