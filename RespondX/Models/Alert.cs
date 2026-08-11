using System;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class Alert
    {
        public int AlertID { get; set; }

        public int? LearnerID { get; set; }

        public int? AdminID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Alert Type")]
        public string AlertType { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }

        [Display(Name = "Priority")]
        [Range(1, 5)]
        public int Priority { get; set; }

        [Display(Name = "Is Read")]
        public bool IsRead { get; set; }

        [Display(Name = "Is Acknowledged")]
        public bool IsAcknowledged { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        // Navigation properties
        public virtual User Learner { get; set; }
        public virtual User Admin { get; set; }

        // Computed properties
        [Display(Name = "Priority Level")]
        public string PriorityLevel
        {
            get
            {
                switch (Priority)
                {
                    case 5: return "Critical";
                    case 4: return "High";
                    case 3: return "Medium";
                    case 2: return "Low";
                    case 1: return "Very Low";
                    default: return "Not Set";
                }
            }
        }

        [Display(Name = "Alert Type Display")]
        public string AlertTypeDisplay
        {
            get
            {
                switch (AlertType)
                {
                    case "Emergency": return "Emergency";
                    case "Reminder": return "Reminder";
                    case "Notification": return "Notification";
                    case "System": return "System";
                    default: return AlertType;
                }
            }
        }

        [Display(Name = "Status")]
        public string Status
        {
            get
            {
                if (IsAcknowledged) return "Acknowledged";
                if (IsRead) return "Read";
                return "Unread";
            }
        }

        [Display(Name = "Is Expired")]
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.Now;

        [Display(Name = "Is Active")]
        public bool IsActive => !IsExpired && !IsAcknowledged;

        [Display(Name = "Time Ago")]
        public string TimeAgo
        {
            get
            {
                var diff = DateTime.Now - CreatedAt;
                if (diff.TotalMinutes < 1) return "Just now";
                if (diff.TotalMinutes < 60) return $"{Math.Floor(diff.TotalMinutes)} minutes ago";
                if (diff.TotalHours < 24) return $"{Math.Floor(diff.TotalHours)} hours ago";
                return CreatedAt.ToString("MMM dd, yyyy");
            }
        }

        [Display(Name = "Priority Class")]
        public string PriorityClass
        {
            get
            {
                switch (Priority)
                {
                    case 5: return "critical";
                    case 4: return "high";
                    case 3: return "medium";
                    default: return "low";
                }
            }
        }
    }
}