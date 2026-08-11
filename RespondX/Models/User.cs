using System;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(50)]
        public string Salt { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLogin { get; set; }

        [StringLength(255)]
        public string ProfileImage { get; set; }

        // Computed properties
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Is Administrator")]
        public bool IsAdmin => Role == "Admin";

        [Display(Name = "Is Expert")]
        public bool IsExpert => Role == "Expert";

        [Display(Name = "Is Learner")]
        public bool IsLearner => Role == "Learner";

        [Display(Name = "Account Status")]
        public string AccountStatus => IsActive ? "Active" : "Inactive";

        [Display(Name = "Last Login")]
        public string LastLoginDisplay => LastLogin?.ToString("MMM dd, yyyy HH:mm") ?? "Never";
    }
}