using System;
using System.Collections.Generic;
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
                    LoadQuiz(quizId);
                }
                else
                {
                    pnlQuiz.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadQuiz(int quizId)
        {
            var quiz = GetQuiz(quizId);
            if (quiz == null)
            {
                pnlQuiz.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            pnlQuiz.Visible = true;
            lblQuizTitle.Text = quiz.Title;
            lblQuizDescription.Text = quiz.Description;
            lblTimeLimit.Text = quiz.TimeLimit.ToString();
            lblTotalQuestions.Text = quiz.Questions.Count.ToString();
            lblPassingScore.Text = quiz.PassingScore.ToString();
            lblTotalSeconds.Text = (quiz.TimeLimit * 60).ToString();

            LoadQuestions(quiz.Questions);
            LoadQuestionNav(quiz.Questions);
        }

        private QuizItem GetQuiz(int quizId)
        {
            return new QuizItem
            {
                QuizID = 1,
                Title = "Emergency Response Fundamentals",
                Description = "Test your knowledge of emergency response basics.",
                TimeLimit = 30,
                PassingScore = 70,
                Questions = new List<QuestionItem>
                {
                    new QuestionItem
                    {
                        QuestionID = 1,
                        QuestionText = "What is the first step in emergency response?",
                        Points = 5,
                        Options = new List<OptionItem>
                        {
                            new OptionItem { OptionID = 1, OptionText = "Call 911", OptionLabel = "A", IsCorrect = true },
                            new OptionItem { OptionID = 2, OptionText = "Run away", OptionLabel = "B", IsCorrect = false },
                            new OptionItem { OptionID = 3, OptionText = "Wait for help", OptionLabel = "C", IsCorrect = false },
                            new OptionItem { OptionID = 4, OptionText = "Panic", OptionLabel = "D", IsCorrect = false }
                        }
                    },
                    new QuestionItem
                    {
                        QuestionID = 2,
                        QuestionText = "Which of the following is a sign of a medical emergency?",
                        Points = 5,
                        Options = new List<OptionItem>
                        {
                            new OptionItem { OptionID = 5, OptionText = "Chest pain", OptionLabel = "A", IsCorrect = true },
                            new OptionItem { OptionID = 6, OptionText = "Hunger", OptionLabel = "B", IsCorrect = false },
                            new OptionItem { OptionID = 7, OptionText = "Sleepiness", OptionLabel = "C", IsCorrect = false },
                            new OptionItem { OptionID = 8, OptionText = "Thirst", OptionLabel = "D", IsCorrect = false }
                        }
                    }
                }
            };
        }

        private void LoadQuestions(List<QuestionItem> questions)
        {
            rptQuestions.DataSource = questions;
            rptQuestions.DataBind();
        }

        private void LoadQuestionNav(List<QuestionItem> questions)
        {
            rptQuestionNav.DataSource = questions;
            rptQuestionNav.DataBind();
        }

        protected void btnSubmitQuiz_Click(object sender, EventArgs e)
        {
            int score = 75;
            int percentage = 75;
            Response.Redirect($"QuizResult.aspx?id={1}&score={score}&percentage={percentage}");
        }
    }
}