using FairPlaySocial.Models.Notifications;

namespace FairPlaySocial.Notifications.Hubs
{
    public interface IPostNotificationHub
    {
        Task ReceiveMessage(PostNotificationModel model);
    }
}
