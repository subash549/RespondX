using System;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class EmergencyContact
    {
        public int ContactID { get; set; }

        [Required]
        public int LearnerID { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Relationship is required")]
        [StringLength(50)]
        public string Relationship { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        public string Email { get; set; }

        [Display(Name = "Is Primary")]
        public bool IsPrimary { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public virtual Learner Learner { get; set; }

        // Computed properties
        [Display(Name = "Contact Type")]
        public string ContactType => IsPrimary ? "Primary Contact" : "Secondary Contact";

        [Display(Name = "Is Complete")]
        public bool IsComplete => !string.IsNullOrEmpty(FullName) &&
                                  !string.IsNullOrEmpty(Relationship) &&
                                  !string.IsNullOrEmpty(PhoneNumber);

        [Display(Name = "Display Name")]
        public string DisplayName => $"{FullName} ({Relationship})";
    }
}