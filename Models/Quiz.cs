using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class Quiz
    {
        public int QuizID { get; set; }

        [Required]
        public int ModuleID { get; set; }

        [Required(ErrorMessage = "Quiz title is required")]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "Time Limit (Minutes)")]
        [Range(1, 180, ErrorMessage = "Time limit must be between 1 and 180 minutes")]
        public int TimeLimitMinutes { get; set; }

        [Display(Name = "Passing Score (%)")]
        [Range(0, 100, ErrorMessage = "Passing score must be between 0 and 100")]
        public int PassingScore { get; set; }

        [Display(Name = "Max Attempts")]
        [Range(1, 10, ErrorMessage = "Maximum attempts must be between 1 and 10")]
        public int MaxAttempts { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Module Module { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<QuizAttempt> Attempts { get; set; }

        // Computed properties
        [Display(Name = "Status")]
        public string Status => IsActive ? "Active" : "Inactive";

        [Display(Name = "Total Questions")]
        public int TotalQuestions => Questions?.Count ?? 0;

        [Display(Name = "Total Points")]
        public int TotalPoints
        {
            get
            {
                int total = 0;
                if (Questions != null)
                {
                    foreach (var question in Questions)
                    {
                        total += question.Points;
                    }
                }
                return total;
            }
        }

        [Display(Name = "Time Limit")]
        public string TimeLimitDisplay
        {
            get
            {
                if (TimeLimitMinutes <= 0) return "No time limit";
                if (TimeLimitMinutes < 60) return $"{TimeLimitMinutes} minutes";
                return $"{TimeLimitMinutes / 60}h {TimeLimitMinutes % 60}m";
            }
        }

        [Display(Name = "Average Score")]
        public decimal? AverageScore
        {
            get
            {
                if (Attempts == null || Attempts.Count == 0) return null;
                decimal total = 0;
                int count = 0;
                foreach (var attempt in Attempts)
                {
                    if (attempt.PercentageScore.HasValue)
                    {
                        total += attempt.PercentageScore.Value;
                        count++;
                    }
                }
                return count > 0 ? total / count : (decimal?)null;
            }
        }
    }
}