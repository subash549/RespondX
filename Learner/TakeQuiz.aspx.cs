using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class TakeQuiz : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                int quizId;
                if (int.TryParse(Request.QueryString["id"], out quizId))
                {
                    var quiz = GetQuiz(quizId);
                    int learnerId = SessionHelper.GetCurrentUserId().GetValueOrDefault();
                    if (quiz == null)
                    {
                        pnlQuiz.Visible = false;
                        pnlLocked.Visible = false;
                        pnlNoQuestions.Visible = false;
                        pnlNotFound.Visible = true;
                    }
                    else if (quiz.ModuleID <= 0 || !QuizAccessHelper.IsModuleCompleted(quiz.ModuleID, learnerId))
                    {
                        pnlQuiz.Visible = false;
                        pnlNoQuestions.Visible = false;
                        pnlNotFound.Visible = false;
                        pnlLocked.Visible = true;
                        lblLockedModule.Text = Server.HtmlEncode(quiz.Module);
                    }
                    else if (quiz.Questions.Count == 0)
                    {
                        pnlQuiz.Visible = false;
                        pnlLocked.Visible = false;
                        pnlNotFound.Visible = false;
                        pnlNoQuestions.Visible = true;
                    }
                    else
                    {
                        LoadQuiz(quiz);
                    }
                }
                else
                {
                    pnlQuiz.Visible = false;
                    pnlLocked.Visible = false;
                    pnlNoQuestions.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadQuiz(QuizItem quiz)
        {
            pnlQuiz.Visible = true;
            lblQuizTitle.Text = quiz.Title;
            lblQuizDescription.Text = quiz.Description;
            lblTimeLimit.Text = quiz.TimeLimit.ToString();
            lblTotalQuestions.Text = quiz.Questions.Count.ToString();
            lblPassingScore.Text = quiz.PassingScore.ToString();
            lblTotalSeconds.Text = (quiz.TimeLimit * 60).ToString();
            litQuestionCount.Text = quiz.Questions.Count.ToString();

            LoadQuestions(quiz.Questions);
            LoadQuestionNav(quiz.Questions);
        }

        private QuizItem GetQuiz(int quizId)
        {
            if (quizId < 0)
            {
                var definition = QuizCatalog.GetById(-quizId);
                if (definition == null)
                    return null;

                int moduleId = QuizAccessHelper.GetModuleIdByTitle(definition.ModuleTitle);
                if (moduleId <= 0)
                    return null;

                return new QuizItem
                {
                    QuizID = quizId,
                    ModuleID = moduleId,
                    Title = definition.Title,
                    Module = definition.ModuleTitle,
                    Description = definition.Description,
                    TimeLimit = definition.TimeLimitMinutes,
                    TimeLimitMinutes = definition.TimeLimitMinutes,
                    PassingScore = definition.PassingScore,
                    MaxAttempts = definition.MaxAttempts,
                    IsActive = true,
                    Questions = QuizQuestionBank.GetQuestions(definition.QuizID)
                };
            }

            QuizItem quiz = null;
            using (var connection = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var command = new SqlCommand(@"
                SELECT q.QuizID, q.ModuleID, q.Title, q.Description,
                       q.TimeLimitMinutes, q.PassingScore, q.MaxAttempts, m.Title AS ModuleTitle
                FROM dbo.Quizzes q
                INNER JOIN dbo.Modules m ON m.ModuleID = q.ModuleID
                WHERE q.QuizID = @QuizID AND q.IsActive = 1 AND m.IsActive = 1;", connection))
            {
                command.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizId;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        quiz = new QuizItem
                        {
                            QuizID = Convert.ToInt32(reader["QuizID"]),
                            ModuleID = Convert.ToInt32(reader["ModuleID"]),
                            Title = Convert.ToString(reader["Title"]),
                            Module = Convert.ToString(reader["ModuleTitle"]),
                            Description = reader["Description"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Description"]),
                            TimeLimit = Convert.ToInt32(reader["TimeLimitMinutes"]),
                            TimeLimitMinutes = Convert.ToInt32(reader["TimeLimitMinutes"]),
                            PassingScore = Convert.ToInt32(reader["PassingScore"]),
                            MaxAttempts = Convert.ToInt32(reader["MaxAttempts"]),
                            IsActive = true,
                            Questions = new List<QuestionItem>()
                        };
                    }
                }
            }

            if (quiz == null)
                return null;

            var builtInQuiz = QuizCatalog.GetAll().Find(definition =>
                string.Equals(definition.ModuleTitle.Trim(), quiz.Module.Trim(), StringComparison.OrdinalIgnoreCase));
            if (builtInQuiz != null)
                quiz.Questions.AddRange(QuizQuestionBank.GetQuestions(builtInQuiz.QuizID));
            quiz.Questions.AddRange(QuizQuestionRepository.GetActiveQuestionsForQuiz(quizId));
            return quiz;
        }

        private void LoadQuestions(List<QuestionItem> questions)
        {
            foreach (var question in questions)
            {
                foreach (var option in question.Options)
                    option.QuestionID = question.QuestionID;
            }

            rptQuestions.DataSource = questions;
            rptQuestions.DataBind();
        }

        private void LoadQuestionNav(List<QuestionItem> questions)
        {
            rptQuestionNav.DataSource = questions;
            rptQuestionNav.DataBind();
        }

        protected bool IsOptionSelected(object questionIdValue, object optionIdValue)
        {
            var questionId = Convert.ToString(questionIdValue);
            var postedOptionId = Request.Form["question_" + questionId];
            return !string.IsNullOrEmpty(postedOptionId) &&
                string.Equals(postedOptionId, Convert.ToString(optionIdValue), StringComparison.Ordinal);
        }

        protected void btnSubmitQuiz_Click(object sender, EventArgs e)
        {
            int quizId;
            var quiz = int.TryParse(Request.QueryString["id"], out quizId) ? GetQuiz(quizId) : null;
            var learnerId = SessionHelper.GetCurrentUserId();
            int moduleId = quiz == null ? 0 : quiz.ModuleID;
            if (quiz == null || !learnerId.HasValue || moduleId <= 0 || quiz.Questions.Count == 0 ||
                !QuizAccessHelper.IsModuleCompleted(moduleId, learnerId.Value))
            {
                Response.Redirect("~/Learner/Quizzes.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            var selectedOptions = new Dictionary<int, OptionItem>();
            int unansweredCount = 0;
            int firstUnansweredIndex = -1;
            for (int questionIndex = 0; questionIndex < quiz.Questions.Count; questionIndex++)
            {
                var question = quiz.Questions[questionIndex];
                var selectedOption = GetSubmittedOption(question);
                if (selectedOption == null)
                {
                    unansweredCount++;
                    if (firstUnansweredIndex < 0)
                        firstUnansweredIndex = questionIndex;
                }
                else
                {
                    selectedOptions[question.QuestionID] = selectedOption;
                }
            }

            if (unansweredCount > 0)
            {
                LoadQuiz(quiz);
                quizValidationMessage.InnerText = "Please answer every question before submitting. You still have " +
                    unansweredCount + " unanswered question(s).";
                quizValidationMessage.Style["display"] = "block";
                ClientScript.RegisterStartupScript(this.GetType(), "incomplete-quiz-question",
                    "window.initialQuizQuestionIndex = " + firstUnansweredIndex + ";", true);
                return;
            }

            int earnedPoints = 0;
            int totalPoints = 0;
            int correctAnswers = 0;
            var answerReview = new List<AnswerReviewItem>();

            foreach (var question in quiz.Questions)
            {
                totalPoints += question.Points;
                var selectedOption = selectedOptions[question.QuestionID];
                var correctOption = question.Options.Find(option => option.IsCorrect);
                bool isCorrect = selectedOption != null && selectedOption.IsCorrect;

                if (isCorrect)
                {
                    earnedPoints += question.Points;
                    correctAnswers++;
                }

                answerReview.Add(new AnswerReviewItem
                {
                    QuestionText = question.QuestionText,
                    SelectedAnswer = selectedOption == null ? "Not answered" : selectedOption.OptionText,
                    CorrectAnswer = correctOption == null ? string.Empty : correctOption.OptionText,
                    IsCorrect = isCorrect,
                    Explanation = isCorrect ? string.Empty : "Review the lesson material and try again."
                });
            }

            var result = new QuizResultSnapshot
            {
                QuizID = quizId,
                LearnerID = learnerId.Value,
                ModuleID = moduleId,
                ModuleTitle = quiz.Module,
                CorrectAnswers = correctAnswers,
                TotalQuestions = quiz.Questions.Count,
                PercentageScore = totalPoints == 0 ? 0 : Math.Round((decimal)earnedPoints * 100m / totalPoints, 2),
                PassingScore = quiz.PassingScore,
                TimeTaken = "Not recorded",
                Answers = answerReview,
                CompletedAllQuestions = true
            };

            Session[GetResultSessionKey(learnerId.Value, quizId)] = result;
            Response.Redirect($"~/Learner/QuizResult.aspx?id={quizId}", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private OptionItem GetSubmittedOption(QuestionItem question)
        {
            int selectedOptionId;
            return int.TryParse(Request.Form["question_" + question.QuestionID], out selectedOptionId)
                ? question.Options.Find(option => option.OptionID == selectedOptionId)
                : null;
        }

        internal static string GetResultSessionKey(int learnerId, int quizId)
        {
            return $"RespondX.QuizResult.{learnerId}.{quizId}";
        }
    }
}
