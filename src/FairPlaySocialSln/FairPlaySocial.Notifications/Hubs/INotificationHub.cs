using FairPlaySocial.Models.Notifications;

namespace FairPlaySocial.Notifications.Hubs
{
    public interface INotificationHub
    {
        Task ReceiveMessage(NotificationModel model);
    }
}
