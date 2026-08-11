using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class ScenarioOption
    {
        public int OptionID { get; set; }

        [Required]
        public int ScenarioID { get; set; }

        [Required(ErrorMessage = "Option text is required")]
        public string OptionText { get; set; }

        [Display(Name = "Is Correct")]
        public bool IsCorrect { get; set; }

        [StringLength(500)]
        public string Feedback { get; set; }

        [Required]
        [Display(Name = "Option Order")]
        [Range(1, int.MaxValue)]
        public int OptionOrder { get; set; }

        // Navigation property
        public virtual Scenario Scenario { get; set; }

        // Computed properties
        [Display(Name = "Type")]
        public string OptionType => IsCorrect ? "Correct Answer" : "Incorrect Answer";

        [Display(Name = "Has Feedback")]
        public bool HasFeedback => !string.IsNullOrEmpty(Feedback);
    }
}