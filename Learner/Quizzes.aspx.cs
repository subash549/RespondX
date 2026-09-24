using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;

namespace RespondX.Learner
{
    public partial class Quizzes : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                LoadQuizzes();
            }
        }

        private void LoadQuizzes()
        {
            var quizzes = GetQuizzes();

            // Apply filters
            var status = ddlStatus.SelectedValue;
            if (status != "All")
            {
                quizzes = quizzes.FindAll(q => q.Status == status);
            }

            // Apply search
            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                quizzes = quizzes.FindAll(q =>
                    q.Title.ToLower().Contains(search.ToLower()) ||
                    q.Description.ToLower().Contains(search.ToLower())
                );
            }

            rptQuizzes.DataSource = quizzes;
            rptQuizzes.DataBind();
        }

        private List<Quiz> GetQuizzes()
        {
            // In production, load from database with user progress
            return new List<Quiz>
            {
                new Quiz
                {
                    QuizID = 1,
                    Title = "Emergency Response Fundamentals",
                    Module = "Introduction to Emergency Response",
                    Description = "Test your knowledge of emergency response basics and procedures.",
                    TimeLimit = 30,
                    QuestionCount = 20,
                    PassingScore = 70,
                    Attempts = 0,
                    MaxAttempts = 3,
                    Status = "Available",
                    StatusBadge = "badge-success",
                    ButtonText = "Start Quiz",
                    ButtonClass = "btn btn-primary btn-sm",
                    IsAvailable = true,
                    IsCompleted = false,
                    Score = null
                },
                new Quiz
                {
                    QuizID = 2,
                    Title = "CPR and First Aid Certification",
                    Module = "CPR and First Aid",
                    Description = "Comprehensive assessment of CPR and first aid knowledge.",
                    TimeLimit = 45,
                    QuestionCount = 30,
                    PassingScore = 80,
                    Attempts = 1,
                    MaxAttempts = 3,
                    Status = "In Progress",
                    StatusBadge = "badge-warning",
                    ButtonText = "Continue",
                    ButtonClass = "btn btn-primary btn-sm",
                    IsAvailable = true,
                    IsCompleted = false,
                    Score = 65
                },
                new Quiz
                {
                    QuizID = 3,
                    Title = "Emergency Communication",
                    Module = "Emergency Communication",
                    Description = "Test your communication skills in emergency situations.",
                    TimeLimit = 20,
                    QuestionCount = 15,
                    PassingScore = 70,
                    Attempts = 2,
                    MaxAttempts = 2,
                    Status = "Completed",
                    StatusBadge = "badge-success",
                    ButtonText = "Retake",
                    ButtonClass = "btn btn-secondary btn-sm",
                    IsAvailable = false,
                    IsCompleted = true,
                    Score = 85
                }
            };
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadQuizzes();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadQuizzes();
        }

        protected void btnTakeQuiz_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                int quizId = int.Parse(btn.CommandArgument);
                Response.Redirect($"TakeQuiz.aspx?id={quizId}");
            }
        }

        protected void btnReview_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                int quizId = int.Parse(btn.CommandArgument);
                Response.Redirect($"QuizResult.aspx?id={quizId}");
            }
        }
    }

    public class Quiz
    {
        public int QuizID { get; set; }
        public string Title { get; set; }
        public string Module { get; set; }
        public string Description { get; set; }
        public int TimeLimit { get; set; }
        public int QuestionCount { get; set; }
        public int PassingScore { get; set; }
        public int Attempts { get; set; }
        public int MaxAttempts { get; set; }
        public string Status { get; set; }
        public string StatusBadge { get; set; }
        public string ButtonText { get; set; }
        public string ButtonClass { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsCompleted { get; set; }
        public int? Score { get; set; }
    }
}