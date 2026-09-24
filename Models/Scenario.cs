using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class Scenario
    {
        public int ScenarioID { get; set; }

        [Required]
        public int ModuleID { get; set; }

        [Required(ErrorMessage = "Scenario title is required")]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Scenario text is required")]
        public string ScenarioText { get; set; }

        [Required]
        [Display(Name = "Scenario Order")]
        [Range(1, int.MaxValue)]
        public int ScenarioOrder { get; set; }

        [Display(Name = "Difficulty Level")]
        [Range(1, 5, ErrorMessage = "Difficulty must be between 1 and 5")]
        public int DifficultyLevel { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Module Module { get; set; }
        public virtual ICollection<ScenarioOption> Options { get; set; }

        // Computed properties
        [Display(Name = "Status")]
        public string Status => IsActive ? "Active" : "Inactive";

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