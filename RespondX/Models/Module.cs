using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class Module
    {
        public int ModuleID { get; set; }

        [Required(ErrorMessage = "Module title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Module Order")]
        [Range(1, int.MaxValue, ErrorMessage = "Module order must be at least 1")]
        public int ModuleOrder { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Estimated Hours")]
        [Range(0.5, 40, ErrorMessage = "Estimated hours must be between 0.5 and 40")]
        public int EstimatedHours { get; set; }

        [Display(Name = "Is Mandatory")]
        public bool IsMandatory { get; set; }

        // Navigation properties
        public virtual User Creator { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Scenario> Scenarios { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
        public virtual ICollection<LearnerProgress> Progress { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; }

        // Computed properties
        [Display(Name = "Status")]
        public string Status => IsActive ? "Active" : "Inactive";

        [Display(Name = "Lesson Count")]
        public int LessonCount => Lessons?.Count ?? 0;

        [Display(Name = "Total Quiz Points")]
        public int TotalQuizPoints
        {
            get
            {
                int total = 0;
                if (Quizzes != null)
                {
                    foreach (var quiz in Quizzes)
                    {
                        if (quiz.Questions != null)
                        {
                            foreach (var question in quiz.Questions)
                            {
                                total += question.Points;
                            }
                        }
                    }
                }
                return total;
            }
        }
    }
}