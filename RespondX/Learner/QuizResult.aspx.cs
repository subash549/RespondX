using System;
using System.Collections.Generic;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class QuizResult : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                int quizId;
                int score;
                int percentage;

                if (int.TryParse(Request.QueryString["id"], out quizId) &&
                    int.TryParse(Request.QueryString["score"], out score) &&
                    int.TryParse(Request.QueryString["percentage"], out percentage))
                {
                    LoadResult(quizId, score, percentage);
                }
                else
                {
                    pnlResult.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadResult(int quizId, int score, int percentage)
        {
            pnlResult.Visible = true;
            bool isPassed = percentage >= 70;

            lblResultTitle.Text = isPassed ? "Congratulations!" : "Keep Practicing!";
            lblResultMessage.Text = isPassed ?
                "You have successfully passed this quiz!" :
                "You need more practice. Review the material and try again.";

            lblScore.Text = percentage.ToString();
            lblCorrect.Text = score.ToString();
            lblTotal.Text = "10";
            lblPassingScore.Text = "70";
            lblTimeTaken.Text = "15:30";

            btnCertificate.Visible = isPassed;

            LoadAnswers(quizId);

            string script = $@"
                var ctx = document.getElementById('resultChart').getContext('2d');
                new Chart(ctx, {{
                    type: 'doughnut',
                    data: {{
                        labels: ['Correct', 'Incorrect'],
                        datasets: [{{
                            data: [{score}, {10 - score}],
                            backgroundColor: ['#48bb78', '#fc8181'],
                            borderWidth: 0
                        }}]
                    }},
                    options: {{
                        responsive: true,
                        maintainAspectRatio: true,
                        plugins: {{
                            legend: {{
                                position: 'bottom'
                            }}
                        }}
                    }}
                }});
            ";

            ClientScript.RegisterStartupScript(this.GetType(), "chart", script, true);
        }

        private void LoadAnswers(int quizId)
        {
            var answers = new List<AnswerReviewItem>
            {
                new AnswerReviewItem { QuestionText = "What is the first step in emergency response?", SelectedAnswer = "Call 911", CorrectAnswer = "Call 911", IsCorrect = true, Explanation = "" },
                new AnswerReviewItem { QuestionText = "Which of the following is a sign of a medical emergency?", SelectedAnswer = "Hunger", CorrectAnswer = "Chest pain", IsCorrect = false, Explanation = "Hunger is not typically a sign of a medical emergency. Chest pain is a common sign requiring immediate attention." },
                new AnswerReviewItem { QuestionText = "How often should CPR be performed?", SelectedAnswer = "When the person is unconscious", CorrectAnswer = "When the person is unconscious", IsCorrect = true, Explanation = "" }
            };

            rptAnswers.DataSource = answers;
            rptAnswers.DataBind();
        }

        protected void btnRetake_Click(object sender, EventArgs e)
        {
            Response.Redirect($"TakeQuiz.aspx?id={Request.QueryString["id"]}");
        }

        protected void btnCertificate_Click(object sender, EventArgs e)
        {
            Response.Redirect("Certificates.aspx");
        }
    }
}