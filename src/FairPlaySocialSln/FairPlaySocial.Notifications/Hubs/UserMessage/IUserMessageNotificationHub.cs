using FairPlaySocial.Models.Notifications;
using FairPlaySocial.Models.UserMessage;

namespace FairPlaySocial.Notifications.Hubs.UserMessage
{
    public interface IUserMessageNotificationHub
    {
        Task ReceiveMessage(UserMessageNotificationModel model);
    }
}
