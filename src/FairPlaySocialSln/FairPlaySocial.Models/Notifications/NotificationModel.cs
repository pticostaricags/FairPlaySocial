namespace FairPlaySocial.Models.Notifications
{
    /// <summary>
    /// Rpresents a Notification used in the SignalR communication
    /// </summary>
    public class NotificationModel
    {
        public string? From { get; set; }
        /// <summary>
        /// Message of the SignalR notification
        /// </summary>
        public string? Message { get; set; }
        public string? GroupName { get; set; }
    }
}
