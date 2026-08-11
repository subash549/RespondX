using System;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class ContentReview
    {
        public int ReviewID { get; set; }

        [Required]
        public int ExpertID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Content Type")]
        public string ContentType { get; set; }

        [Required]
        [Display(Name = "Content ID")]
        public int ContentID { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Feedback")]
        public string Feedback { get; set; }

        [Display(Name = "Reviewed At")]
        public DateTime? ReviewedAt { get; set; }

        public DateTime AssignedAt { get; set; }

        [Display(Name = "Priority")]
        [Range(1, 5)]
        public int Priority { get; set; }

        // Navigation property
        public virtual User Expert { get; set; }

        // Computed properties
        [Display(Name = "Status Display")]
        public string StatusDisplay
        {
            get
            {
                switch (Status)
                {
                    case "Pending": return "Pending Review";
                    case "Approved": return "Approved";
                    case "Rejected": return "Rejected";
                    case "NeedsRevision": return "Needs Revision";
                    default: return Status;
                }
            }
        }

        [Display(Name = "Status Class")]
        public string StatusClass
        {
            get
            {
                switch (Status)
                {
                    case "Pending": return "pending";
                    case "Approved": return "approved";
                    case "Rejected": return "rejected";
                    case "NeedsRevision": return "needs-revision";
                    default: return "default";
                }
            }
        }

        [Display(Name = "Content Type Display")]
        public string ContentTypeDisplay
        {
            get
            {
                switch (ContentType)
                {
                    case "Module": return "Module";
                    case "Lesson": return "Lesson";
                    case "Quiz": return "Quiz";
                    case "Scenario": return "Scenario";
                    default: return ContentType;
                }
            }
        }

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

        [Display(Name = "Is Completed")]
        public bool IsCompleted => Status != "Pending";

        [Display(Name = "Is Approved")]
        public bool IsApproved => Status == "Approved";

        [Display(Name = "Assigned Date")]
        public string AssignedDateDisplay => AssignedAt.ToString("MMM dd, yyyy HH:mm");

        [Display(Name = "Review Date")]
        public string ReviewDateDisplay => ReviewedAt?.ToString("MMM dd, yyyy HH:mm") ?? "Not reviewed";

        [Display(Name = "Time to Review")]
        public string TimeToReview
        {
            get
            {
                if (!ReviewedAt.HasValue) return "Not reviewed";
                var diff = ReviewedAt.Value - AssignedAt;
                if (diff.TotalMinutes < 60) return $"{Math.Floor(diff.TotalMinutes)} minutes";
                if (diff.TotalHours < 24) return $"{Math.Floor(diff.TotalHours)} hours";
                return $"{Math.Floor(diff.TotalDays)} days";
            }
        }

        [Display(Name = "Has Feedback")]
        public bool HasFeedback => !string.IsNullOrEmpty(Feedback);

        [Display(Name = "Feedback Preview")]
        public string FeedbackPreview
        {
            get
            {
                if (string.IsNullOrEmpty(Feedback)) return "No feedback provided";
                return Feedback.Length > 100 ? Feedback.Substring(0, 100) + "..." : Feedback;
            }
        }
    }
}