using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class QuizAnswer
    {
        public int AnswerID { get; set; }

        [Required]
        public int AttemptID { get; set; }

        [Required]
        public int QuestionID { get; set; }

        [Display(Name = "Selected Option")]
        public int? SelectedOptionID { get; set; }

        [Display(Name = "Answer Text")]
        public string AnswerText { get; set; }

        [Display(Name = "Is Correct")]
        public bool IsCorrect { get; set; }

        [Display(Name = "Points Earned")]
        [Range(0, int.MaxValue)]
        public int PointsEarned { get; set; }

        // Navigation properties
        public virtual QuizAttempt Attempt { get; set; }
        public virtual Question Question { get; set; }
        public virtual QuizOption SelectedOption { get; set; }

        // Computed properties
        [Display(Name = "Status")]
        public string Status => IsCorrect ? "Correct" : "Incorrect";

        [Display(Name = "Is Answered")]
        public bool IsAnswered => SelectedOptionID.HasValue || !string.IsNullOrEmpty(AnswerText);

        [Display(Name = "Answer Display")]
        public string AnswerDisplay
        {
            get
            {
                if (SelectedOption != null)
                    return SelectedOption.OptionText;
                if (!string.IsNullOrEmpty(AnswerText))
                    return AnswerText;
                return "Not answered";
            }
        }
    }
}