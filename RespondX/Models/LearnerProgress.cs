using System;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class LearnerProgress
    {
        public int ProgressID { get; set; }

        [Required]
        public int LearnerID { get; set; }

        [Required]
        public int ModuleID { get; set; }

        public int? LessonID { get; set; }

        public int? ScenarioID { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? LastAccessedAt { get; set; }

        [Range(0, 100)]
        public int ProgressPercentage { get; set; }

        [Display(Name = "Time Spent (Minutes)")]
        [Range(0, int.MaxValue)]
        public int TimeSpentMinutes { get; set; }

        // Navigation properties
        public virtual Learner Learner { get; set; }
        public virtual Module Module { get; set; }
        public virtual Lesson Lesson { get; set; }
        public virtual Scenario Scenario { get; set; }

        // Computed properties
        [Display(Name = "Status Display")]
        public string StatusDisplay
        {
            get
            {
                switch (Status)
                {
                    case "NotStarted": return "Not Started";
                    case "InProgress": return "In Progress";
                    case "Completed": return "Completed";
                    case "Certified": return "Certified";
                    default: return Status;
                }
            }
        }

        [Display(Name = "Time Spent Display")]
        public string TimeSpentDisplay
        {
            get
            {
                if (TimeSpentMinutes == 0) return "0 minutes";
                if (TimeSpentMinutes < 60) return $"{TimeSpentMinutes} minutes";
                return $"{TimeSpentMinutes / 60}h {TimeSpentMinutes % 60}m";
            }
        }

        [Display(Name = "Is Completed")]
        public bool IsCompleted => Status == "Completed" || Status == "Certified";

        [Display(Name = "Progress Bar")]
        public int ProgressBarWidth => ProgressPercentage;

        [Display(Name = "Completed Date")]
        public string CompletedDateDisplay => CompletedAt?.ToString("MMM dd, yyyy") ?? "Not completed";

        [Display(Name = "Last Activity")]
        public string LastActivityDisplay => LastAccessedAt?.ToString("MMM dd, yyyy HH:mm") ?? "No activity";
    }
}