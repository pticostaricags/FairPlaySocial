using FairPlaySocial.Models.Notifications;

namespace FairPlaySocial.Notifications.Hubs.Post
{
    public interface IPostNotificationHub
    {
        Task ReceiveMessage(PostNotificationModel model);
    }
}
