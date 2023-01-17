using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Models.Notifications
{
    public class UserMessageNotificationModel
    {
        /// <summary>
        /// Message of the SignalR notification
        /// </summary>
        public string? Message { get; set; }
        public string? GroupName { get; set; }
    }
}
