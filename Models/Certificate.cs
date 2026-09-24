using System;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class Certificate
    {
        public int CertificateID { get; set; }

        [Required]
        public int LearnerID { get; set; }

        [Required]
        public int ModuleID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Certificate Number")]
        public string CertificateNumber { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [StringLength(255)]
        [Display(Name = "PDF Path")]
        public string PdfPath { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Verification Code")]
        public string VerificationCode { get; set; }

        [Display(Name = "Score")]
        [Range(0, 100)]
        public decimal Score { get; set; }

        // Navigation properties
        public virtual Learner Learner { get; set; }
        public virtual Module Module { get; set; }

        // Computed properties (for display)
        [Display(Name = "Learner Name")]
        public string LearnerName { get; set; }

        [Display(Name = "Module Title")]
        public string ModuleTitle { get; set; }

        [Display(Name = "Status")]
        public string Status
        {
            get
            {
                if (!IsActive) return "Revoked";
                if (ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now) return "Expired";
                return "Valid";
            }
        }

        [Display(Name = "Is Valid")]
        public bool IsValid => IsActive && (!ExpiryDate.HasValue || ExpiryDate.Value >= DateTime.Now);

        [Display(Name = "Issue Date Display")]
        public string IssueDateDisplay => IssueDate.ToString("MMMM dd, yyyy");

        [Display(Name = "Expiry Date Display")]
        public string ExpiryDateDisplay => ExpiryDate?.ToString("MMMM dd, yyyy") ?? "No Expiry";

        [Display(Name = "Days Valid")]
        public int? DaysValid
        {
            get
            {
                if (!ExpiryDate.HasValue) return null;
                return (int)(ExpiryDate.Value - IssueDate).TotalDays;
            }
        }

        [Display(Name = "Days Remaining")]
        public int? DaysRemaining
        {
            get
            {
                if (!ExpiryDate.HasValue || !IsValid) return null;
                return (int)(ExpiryDate.Value - DateTime.Now).TotalDays;
            }
        }

        [Display(Name = "Issuer")]
        public string Issuer { get; set; } = "RespondX Training Institute";
    }
}