using System;
using System.Collections.Generic;

namespace RespondX.Models
{
    // ========== User Models ==========
    public class UserItem
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ========== Module Models ==========
    public class ModuleItem
    {
        public int ModuleID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int LessonCount { get; set; }
        public int EstimatedHours { get; set; }
        public int ModuleOrder { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string StatusBadge { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        
        // New fields
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string ThumbnailUrl { get; set; }
        public string YouTubeVideoUrl { get; set; }
        public string VideoFileUrl { get; set; }
        public int InstructorID { get; set; }
        public string InstructorName { get; set; }
    }

    // ========== Category Models ==========
    public class CategoryItem
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int ModuleCount { get; set; }
    }

    // ========== Assignment Models ==========
    public class AssignmentItem
    {
        public int AssignmentID { get; set; }
        public int ModuleID { get; set; }
        public string ModuleTitle { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxScore { get; set; }
        public bool IsActive { get; set; }
        public int SubmissionsCount { get; set; }
    }

    public class AssignmentSubmissionItem
    {
        public int SubmissionID { get; set; }
        public int AssignmentID { get; set; }
        public string AssignmentTitle { get; set; }
        public int LearnerID { get; set; }
        public string LearnerName { get; set; }
        public string Content { get; set; }
        public string FileUrl { get; set; }
        public int? Score { get; set; }
        public string Feedback { get; set; }
        public string Status { get; set; } // Pending, Graded
        public DateTime SubmittedAt { get; set; }
        public string StatusBadge { get; set; }
    }

    // ========== Lesson Models ==========
    public class LessonItem
    {
        public int LessonID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int LessonOrder { get; set; }
        public string VideoUrl { get; set; }
        public string ResourceUrl { get; set; }
        public bool IsActive { get; set; }
        public bool HasVideo { get; set; }
        public bool HasResources { get; set; }
        public string ModuleTitle { get; set; }
        public int ModuleId { get; set; }
        public int EstimatedTime { get; set; }
        public bool IsCompleted { get; set; }
        public string Description { get; set; }
    }

    // ========== Scenario Models ==========
    public class ScenarioItem
    {
        public int ScenarioID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ScenarioText { get; set; }
        public int ScenarioOrder { get; set; }
        public int DifficultyLevel { get; set; }
        public string DifficultyDisplay { get; set; }
        public int OptionsCount { get; set; }
        public bool IsActive { get; set; }
        public string Module { get; set; }
        public string Difficulty { get; set; }
        public string DifficultyClass { get; set; }
        public int TimeEstimate { get; set; }
        public string Status { get; set; }
        public string StatusBadge { get; set; }
        public int Attempts { get; set; }
        public bool HasOptions { get; set; }
    }

    public class ScenarioOptionItem
    {
        public int OptionID { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
        public string Feedback { get; set; }
        public int OptionOrder { get; set; }
    }

    // ========== Quiz Models ==========
    public class QuizItem
    {
        public int QuizID { get; set; }
        public int ModuleID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TimeLimit { get; set; }
        public int PassingScore { get; set; }
        public int MaxAttempts { get; set; }
        public int QuestionCount { get; set; }
        public bool IsActive { get; set; }
        public string Module { get; set; }
        public int Attempts { get; set; }
        public string Status { get; set; }
        public string StatusBadge { get; set; }
        public string ButtonText { get; set; }
        public string ButtonClass { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsCompleted { get; set; }
        public int? Score { get; set; }
        public int TimeLimitMinutes { get; set; }
        public List<QuestionItem> Questions { get; set; }
    }

    public class QuestionItem
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string QuestionTypeDisplay { get; set; }
        public int Points { get; set; }
        public bool IsActive { get; set; }
        public List<OptionItem> Options { get; set; }
    }

    public class OptionItem
    {
        public int OptionID { get; set; }
        public int QuestionID { get; set; }
        public string OptionText { get; set; }
        public string OptionLabel { get; set; }
        public bool IsCorrect { get; set; }
        public int OptionOrder { get; set; }
    }

    // ========== Alert Models ==========
    public class AlertItem
    {
        public int AlertID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string AlertType { get; set; }
        public string AlertTypeDisplay { get; set; }
        public string TypeBadge { get; set; }
        public int Priority { get; set; }
        public string PriorityLevel { get; set; }
        public string PriorityClass { get; set; }
        public string Target { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; }
        public bool IsRead { get; set; }
        public bool IsAcknowledged { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int RecipientCount { get; set; }
        public int ReadCount { get; set; }
    }

    // ========== Review Models ==========
    public class PendingReviewItem
    {
        public int ReviewID { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string SubmittedBy { get; set; }
        public string AssignedAt { get; set; }
        public string DueDate { get; set; }
        public string Priority { get; set; }
        public string PriorityClass { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string StatusBadge { get; set; }
        public string TimeAgo { get; set; }
    }

    public class ReviewContentItem
    {
        public int ReviewID { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string SubmittedBy { get; set; }
        public string Priority { get; set; }
        public string PriorityClass { get; set; }
        public string Content { get; set; }
    }

    public class HistoryItem
    {
        public int ReviewID { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string Status { get; set; }
        public int Rating { get; set; }
        public string ReviewedAt { get; set; }
    }

    // ========== Certificate Models ==========
    public class CertificateItem
    {
        public int CertificateID { get; set; }
        public string ModuleTitle { get; set; }
        public string LearnerName { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal Score { get; set; }
        public string VerificationCode { get; set; }
        public bool IsValid { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string DownloadUrl { get; set; }
        public string ViewUrl { get; set; }
    }

    // ========== Contact Models ==========
    public class ContactItem
    {
        public int ContactID { get; set; }
        public int LearnerID { get; set; }
        public string LearnerName { get; set; }
        public string FullName { get; set; }
        public string Relationship { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsPrimary { get; set; }
    }

    // ========== Activity Models ==========
    public class ActivityItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TimeAgo { get; set; }
        public string ActivityType { get; set; }
        public string Icon { get; set; }
    }

    // ========== Resource Models ==========
    public class ResourceItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
    }

    // ========== Progress Models ==========
    public class ModuleProgressItem
    {
        public string ModuleTitle { get; set; }
        public string Status { get; set; }
        public string StatusBadge { get; set; }
        public int ProgressPercentage { get; set; }
        public string TimeSpent { get; set; }
        public string LastActivity { get; set; }
    }

    public class AchievementItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool IsEarned { get; set; }
    }

    // ========== Answer Review Models ==========
    [Serializable]
    public class AnswerReviewItem
    {
        public string QuestionText { get; set; }
        public string SelectedAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public string Explanation { get; set; }
    }

    [Serializable]
    public class QuizResultSnapshot
    {
        public int QuizID { get; set; }
        public int LearnerID { get; set; }
        public int ModuleID { get; set; }
        public string ModuleTitle { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public decimal PercentageScore { get; set; }
        public int PassingScore { get; set; }
        public string TimeTaken { get; set; }
        public List<AnswerReviewItem> Answers { get; set; }
        public bool CompletedAllQuestions { get; set; }
    }

    // ========== Report Models ==========
    public class UserReportItem
    {
        public string Role { get; set; }
        public int Total { get; set; }
        public int Active { get; set; }
        public int Inactive { get; set; }
        public int New { get; set; }
    }

    public class ModuleReportItem
    {
        public string ModuleName { get; set; }
        public int Enrollments { get; set; }
        public int CompletionRate { get; set; }
        public int AvgScore { get; set; }
        public string AvgTime { get; set; }
    }

    public class QuizReportItem
    {
        public string QuizName { get; set; }
        public int Attempts { get; set; }
        public int PassRate { get; set; }
        public int AvgScore { get; set; }
        public string AvgTime { get; set; }
    }

    public class CertificateReportItem
    {
        public string Month { get; set; }
        public int Issued { get; set; }
        public int Valid { get; set; }
        public int Expired { get; set; }
        public int Revoked { get; set; }
    }
}
