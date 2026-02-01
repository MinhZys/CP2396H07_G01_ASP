using Symphony.Portal.Web.Models;
using System.Collections.Generic;

namespace Symphony.Portal.Web.Models.ViewModels
{
    public class NotificationBellVM
    {
        public int UnreadCount { get; set; }
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
