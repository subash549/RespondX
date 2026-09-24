using System;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class AssignmentSubmission
    {
        public int SubmissionID { get; set; }

        [Required]
        public int AssignmentID { get; set; }

        [Required]
        public int LearnerID { get; set; }

        public string Content { get; set; }

        public string FileUrl { get; set; }

        public int? Score { get; set; }

        public string Feedback { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // Pending, Graded

        public DateTime SubmittedAt { get; set; }

        // Navigation properties
        public virtual Assignment Assignment { get; set; }
        public virtual Learner Learner { get; set; }
    }
}
