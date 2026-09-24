using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices.CompensatingResourceManager;

namespace RespondX.Models
{
    public class Learner
    {
        public int LearnerID { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(20)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [StringLength(100)]
        public string Organization { get; set; }

        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Display(Name = "Years of Experience")]
        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
        public int ExperienceYears { get; set; }

        public string Certifications { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        // Navigation properties (from User)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }

        // Computed properties
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Age")]
        public int? Age
        {
            get
            {
                if (!DateOfBirth.HasValue) return null;
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        [Display(Name = "Full Address")]
        public string FullAddress => string.IsNullOrEmpty(Address) ? "" : $"{Address}, {City}, {State} {ZipCode}".Trim();

        [Display(Name = "Is Profile Complete")]
        public bool IsProfileComplete => !string.IsNullOrEmpty(PhoneNumber) &&
                                         !string.IsNullOrEmpty(Address) &&
                                         !string.IsNullOrEmpty(City);

        // Navigation properties
        public virtual ICollection<LearnerProgress> Progress { get; set; }
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; }
        public virtual ICollection<EmergencyContact> EmergencyContacts { get; set; }
        public virtual ICollection<Alert> Alerts { get; set; }
    }
}