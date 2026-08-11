using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class Question
    {
        public int QuestionID { get; set; }

        [Required]
        public int QuizID { get; set; }

        [Required(ErrorMessage = "Question text is required")]
        public string QuestionText { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Question Type")]
        public string QuestionType { get; set; }

        [Display(Name = "Difficulty Level")]
        [Range(1, 5)]
        public int DifficultyLevel { get; set; }

        [Display(Name = "Points")]
        [Range(1, 100)]
        public int Points { get; set; }

        [Required]
        [Display(Name = "Question Order")]
        [Range(1, int.MaxValue)]
        public int QuestionOrder { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<QuizOption> Options { get; set; }
        public virtual ICollection<QuizAnswer> Answers { get; set; }

        // Computed properties
        [Display(Name = "Type")]
        public string QuestionTypeDisplay
        {
            get
            {
                switch (QuestionType)
                {
                    case "MultipleChoice": return "Multiple Choice";
                    case "TrueFalse": return "True/False";
                    case "ShortAnswer": return "Short Answer";
                    default: return QuestionType;
                }
            }
        }

        [Display(Name = "Difficulty")]
        public string DifficultyLevelDisplay
        {
            get
            {
                switch (DifficultyLevel)
                {
                    case 1: return "Beginner";
                    case 2: return "Easy";
                    case 3: return "Intermediate";
                    case 4: return "Advanced";
                    case 5: return "Expert";
                    default: return "Not Set";
                }
            }
        }

        [Display(Name = "Options Count")]
        public int OptionsCount => Options?.Count ?? 0;

        [Display(Name = "Has Correct Answer")]
        public bool HasCorrectAnswer
        {
            get
            {
                if (Options == null) return false;
                foreach (var option in Options)
                {
                    if (option.IsCorrect) return true;
                }
                return false;
            }
        }
    }
}