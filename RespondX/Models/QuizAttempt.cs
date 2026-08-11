using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class QuizAttempt
    {
        public int AttemptID { get; set; }

        [Required]
        public int LearnerID { get; set; }

        [Required]
        public int QuizID { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? Score { get; set; }

        [Display(Name = "Percentage Score")]
        public decimal? PercentageScore { get; set; }

        [Display(Name = "Is Passed")]
        public bool IsPassed { get; set; }

        [Required]
        [Display(Name = "Attempt Number")]
        [Range(1, int.MaxValue)]
        public int AttemptNumber { get; set; }

        // Navigation properties
        public virtual Learner Learner { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<QuizAnswer> Answers { get; set; }

        // Computed properties
        [Display(Name = "Duration")]
        public string Duration
        {
            get
            {
                if (!EndTime.HasValue) return "In progress";
                var duration = EndTime.Value - StartTime;
                return $"{duration.Hours}h {duration.Minutes}m {duration.Seconds}s";
            }
        }

        [Display(Name = "Duration (Minutes)")]
        public double DurationMinutes
        {
            get
            {
                if (!EndTime.HasValue) return 0;
                return (EndTime.Value - StartTime).TotalMinutes;
            }
        }

        [Display(Name = "Result")]
        public string ResultDisplay
        {
            get
            {
                if (!EndTime.HasValue) return "In Progress";
                return IsPassed ? "Passed" : "Failed";
            }
        }

        [Display(Name = "Score Display")]
        public string ScoreDisplay
        {
            get
            {
                if (!Score.HasValue || !PercentageScore.HasValue) return "Not graded";
                return $"{Score.Value}/{Quiz?.TotalPoints ?? 0} ({PercentageScore.Value:F1}%)";
            }
        }

        [Display(Name = "Is Complete")]
        public bool IsComplete => EndTime.HasValue;
    }
}