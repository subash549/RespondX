using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
                string selectedStatus = status == "InProgress" ? "In Progress" : status;
                quizzes = quizzes.FindAll(q => q.Status == selectedStatus);
            }

            int moduleId;
            if (int.TryParse(Request.QueryString["moduleId"], out moduleId))
                quizzes = quizzes.FindAll(q => q.ModuleID == moduleId);

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
            var quizzes = new List<Quiz>();
            var learnerId = SessionHelper.GetCurrentUserId();
            if (!learnerId.HasValue)
                return quizzes;

            const string query = @"
                SELECT q.QuizID, q.ModuleID, m.Title AS ModuleTitle, q.Title, q.Description,
                       q.TimeLimitMinutes, q.PassingScore, q.MaxAttempts,
                       (SELECT COUNT(*)
                        FROM dbo.Questions question
                        WHERE question.QuizID = q.QuizID
                          AND question.QuestionType = N'MultipleChoice') AS DatabaseQuestionCount,
                       (SELECT COUNT(*)
                        FROM dbo.QuizAttempts attempt
                        WHERE attempt.QuizID = q.QuizID
                          AND attempt.LearnerID = @LearnerID) AS Attempts,
                       (SELECT TOP 1 CONVERT(INT, ROUND(attempt.PercentageScore, 0))
                        FROM dbo.QuizAttempts attempt
                        WHERE attempt.QuizID = q.QuizID
                          AND attempt.LearnerID = @LearnerID
                          AND attempt.EndTime IS NOT NULL
                          AND attempt.PercentageScore IS NOT NULL
                        ORDER BY attempt.EndTime DESC, attempt.AttemptNumber DESC) AS Score,
                       CAST(CASE WHEN EXISTS
                       (
                           SELECT 1
                           FROM dbo.QuizAttempts attempt
                           WHERE attempt.QuizID = q.QuizID
                             AND attempt.LearnerID = @LearnerID
                             AND attempt.EndTime IS NOT NULL
                       ) THEN 1 ELSE 0 END AS BIT) AS IsCompleted
                FROM dbo.Quizzes q
                INNER JOIN dbo.Modules m ON m.ModuleID = q.ModuleID
                WHERE q.IsActive = 1 AND m.IsActive = 1
                ORDER BY m.ModuleOrder, q.Title;";

            try
            {
                using (var connection = new SqlConnection(DatabaseHelper.ConnectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@LearnerID", SqlDbType.Int).Value = learnerId.Value;
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int moduleId = Convert.ToInt32(reader["ModuleID"]);
                            string moduleTitle = Convert.ToString(reader["ModuleTitle"]);
                            var builtInQuiz = QuizCatalog.GetAll().Find(definition =>
                                string.Equals(definition.ModuleTitle.Trim(), moduleTitle.Trim(), StringComparison.OrdinalIgnoreCase));
                            int questionCount = Convert.ToInt32(reader["DatabaseQuestionCount"]);
                            if (builtInQuiz != null)
                                questionCount += QuizQuestionBank.GetQuestions(builtInQuiz.QuizID).Count;

                            int attempts = Convert.ToInt32(reader["Attempts"]);
                            int maxAttempts = Convert.ToInt32(reader["MaxAttempts"]);
                            bool isCompleted = Convert.ToBoolean(reader["IsCompleted"]);
                            bool moduleCompleted = QuizAccessHelper.IsModuleCompleted(moduleId, learnerId.Value);
                            var quiz = new Quiz
                            {
                                QuizID = Convert.ToInt32(reader["QuizID"]),
                                ModuleID = moduleId,
                                Title = Convert.ToString(reader["Title"]),
                                Module = moduleTitle,
                                Description = reader["Description"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Description"]),
                                TimeLimit = Convert.ToInt32(reader["TimeLimitMinutes"]),
                                QuestionCount = questionCount,
                                PassingScore = Convert.ToInt32(reader["PassingScore"]),
                                Attempts = attempts,
                                MaxAttempts = maxAttempts,
                                ModuleCompleted = moduleCompleted,
                                IsCompleted = isCompleted,
                                Score = reader["Score"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["Score"])
                            };

                            if (!moduleCompleted)
                            {
                                quiz.Status = "Locked";
                                quiz.StatusBadge = "badge-secondary";
                                quiz.ButtonText = "Complete module";
                                quiz.ButtonClass = "btn btn-secondary btn-sm";
                                quiz.IsAvailable = false;
                            }
                            else if (questionCount == 0)
                            {
                                quiz.Status = "Not Ready";
                                quiz.StatusBadge = "badge-secondary";
                                quiz.ButtonText = "No questions yet";
                                quiz.ButtonClass = "btn btn-secondary btn-sm";
                                quiz.IsAvailable = false;
                            }
                            else if (attempts >= maxAttempts)
                            {
                                quiz.Status = isCompleted ? "Completed" : "Attempts Used";
                                quiz.StatusBadge = isCompleted ? "badge-success" : "badge-secondary";
                                quiz.ButtonText = "Attempts used";
                                quiz.ButtonClass = "btn btn-secondary btn-sm";
                                quiz.IsAvailable = false;
                            }
                            else
                            {
                                quiz.Status = isCompleted ? "Completed" : (attempts > 0 ? "In Progress" : "Available");
                                quiz.StatusBadge = isCompleted ? "badge-success" : (attempts > 0 ? "badge-warning" : "badge-primary");
                                quiz.ButtonText = attempts > 0 ? "Retake Quiz" : "Start Quiz";
                                quiz.ButtonClass = "btn btn-primary btn-sm";
                                quiz.IsAvailable = true;
                            }

                            quizzes.Add(quiz);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Load learner quizzes", ex.Message, ex.StackTrace);
            }

            foreach (var definition in QuizCatalog.GetAll())
            {
                int moduleId = QuizAccessHelper.GetModuleIdByTitle(definition.ModuleTitle);
                if (moduleId <= 0 || quizzes.Exists(quiz => quiz.ModuleID == moduleId))
                    continue;

                bool moduleCompleted = QuizAccessHelper.IsModuleCompleted(moduleId, learnerId.Value);
                var catalogQuiz = new Quiz
                {
                    QuizID = -definition.QuizID,
                    ModuleID = moduleId,
                    Title = definition.Title,
                    Module = definition.ModuleTitle,
                    Description = definition.Description,
                    TimeLimit = definition.TimeLimitMinutes,
                    QuestionCount = QuizQuestionBank.GetQuestions(definition.QuizID).Count,
                    PassingScore = definition.PassingScore,
                    Attempts = 0,
                    MaxAttempts = definition.MaxAttempts,
                    IsCompleted = false,
                    ModuleCompleted = moduleCompleted,
                    Score = null
                };

                if (!moduleCompleted)
                {
                    catalogQuiz.Status = "Locked";
                    catalogQuiz.StatusBadge = "badge-secondary";
                    catalogQuiz.ButtonText = "Complete module";
                    catalogQuiz.ButtonClass = "btn btn-secondary btn-sm";
                    catalogQuiz.IsAvailable = false;
                }
                else
                {
                    catalogQuiz.Status = "Available";
                    catalogQuiz.StatusBadge = "badge-primary";
                    catalogQuiz.ButtonText = "Start Quiz";
                    catalogQuiz.ButtonClass = "btn btn-primary btn-sm";
                    catalogQuiz.IsAvailable = true;
                }

                quizzes.Add(catalogQuiz);
            }

            return quizzes;
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
            int quizId;
            if (btn != null && int.TryParse(btn.CommandArgument, out quizId))
            {
                // TakeQuiz repeats the completion check so a crafted postback cannot bypass the lock.
                Response.Redirect($"~/Learner/TakeQuiz.aspx?id={quizId}", false);
                Context.ApplicationInstance.CompleteRequest();
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
        public int ModuleID { get; set; }
        public bool ModuleCompleted { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsCompleted { get; set; }
        public int? Score { get; set; }
    }
}
