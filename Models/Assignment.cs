using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class Assignment
    {
        public int AssignmentID { get; set; }

        [Required]
        public int ModuleID { get; set; }

        [Required(ErrorMessage = "Assignment title is required")]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        [Range(1, 1000)]
        public int MaxScore { get; set; }

        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Module Module { get; set; }
        public virtual ICollection<AssignmentSubmission> Submissions { get; set; }
    }
}
