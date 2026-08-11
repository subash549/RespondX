using System;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class QuizOption
    {
        public int OptionID { get; set; }

        [Required]
        public int QuestionID { get; set; }

        [Required(ErrorMessage = "Option text is required")]
        public string OptionText { get; set; }

        [Display(Name = "Is Correct")]
        public bool IsCorrect { get; set; }

        [Required]
        [Display(Name = "Option Order")]
        [Range(1, int.MaxValue)]
        public int OptionOrder { get; set; }

        // Navigation property
        public virtual Question Question { get; set; }

        // Computed properties
        [Display(Name = "Type")]
        public string OptionType => IsCorrect ? "Correct Answer" : "Incorrect Answer";

        [Display(Name = "Option Label")]
        public string OptionLabel
        {
            get
            {
                char label = (char)('A' + OptionOrder - 1);
                return label.ToString();
            }
        }
    }
}