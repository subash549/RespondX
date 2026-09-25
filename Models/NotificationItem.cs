using System;

namespace RespondX.Models
{
    public class NotificationItem
    {
        public int NotificationID { get; set; }
        public string NotificationType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string TargetUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string TimeAgo
        {
            get
            {
                var elapsed = DateTime.Now - CreatedAt;
                if (elapsed.TotalMinutes < 1) return "Just now";
                if (elapsed.TotalHours < 1) return (int)elapsed.TotalMinutes + "m ago";
                if (elapsed.TotalDays < 1) return (int)elapsed.TotalHours + "h ago";
                return (int)elapsed.TotalDays + "d ago";
            }
        }
    }
}
