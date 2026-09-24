using System;
using System.ComponentModel.DataAnnotations;

namespace RespondX.Models
{
    public class Lesson
    {
        public int LessonID { get; set; }

        [Required]
        public int ModuleID { get; set; }

        [Required(ErrorMessage = "Lesson title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Lesson content is required")]
        public string Content { get; set; }

        [Required]
        [Display(Name = "Lesson Order")]
        [Range(1, int.MaxValue, ErrorMessage = "Lesson order must be at least 1")]
        public int LessonOrder { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Video URL")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string VideoUrl { get; set; }

        [Display(Name = "Resource URL")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string ResourceUrl { get; set; }

        // Navigation property
        public virtual Module Module { get; set; }

        // Computed properties
        [Display(Name = "Status")]
        public string Status => IsActive ? "Active" : "Inactive";

        [Display(Name = "Has Video")]
        public bool HasVideo => !string.IsNullOrEmpty(VideoUrl);

        [Display(Name = "Has Resources")]
        public bool HasResources => !string.IsNullOrEmpty(ResourceUrl);

        [Display(Name = "Content Preview")]
        public string ContentPreview
        {
            get
            {
                if (string.IsNullOrEmpty(Content)) return "";
                return Content.Length > 200 ? Content.Substring(0, 200) + "..." : Content;
            }
        }
    }
}