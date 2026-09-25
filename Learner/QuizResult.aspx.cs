using System;
using System.Web.UI;
using RespondX.Certificates;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class QuizResult : Page
    {
        private const decimal CertificateMinimumScoreExclusive = 80m;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                int quizId;
                var learnerId = SessionHelper.GetCurrentUserId();
                var result = int.TryParse(Request.QueryString["id"], out quizId) && learnerId.HasValue
                    ? Session[TakeQuiz.GetResultSessionKey(learnerId.Value, quizId)] as QuizResultSnapshot
                    : null;

                if (HasCompleteAnswers(result) && result.QuizID == quizId && result.LearnerID == learnerId.Value &&
                    QuizAccessHelper.IsModuleCompleted(result.ModuleID, learnerId.Value))
                {
                    LoadResult(result);
                }
                else
                {
                    pnlResult.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadResult(QuizResultSnapshot result)
        {
            pnlResult.Visible = true;
            bool isPassed = result.PercentageScore >= result.PassingScore;
            bool earnsCertificate = result.PercentageScore > CertificateMinimumScoreExclusive;

            resultHeader.Attributes["class"] = "result-header " + (isPassed ? "passed" : "failed");
            resultIcon.Attributes["class"] = "fas " + (isPassed ? "fa-check-circle" : "fa-times-circle");
            lblResultTitle.Text = isPassed ? "Quiz passed" : "Keep Practicing!";
            lblResultMessage.Text = earnsCertificate
                ? "You scored above 80%. Your certificate is ready to download."
                : isPassed
                    ? "You passed the quiz. A certificate is available only for a score above 80%."
                    : "You need more practice. Review the material and try again.";

            lblScore.Text = result.PercentageScore.ToString("0.##");
            lblCorrect.Text = result.CorrectAnswers.ToString();
            lblTotal.Text = result.TotalQuestions.ToString();
            lblPassingScore.Text = result.PassingScore.ToString();
            lblTimeTaken.Text = result.TimeTaken;
            btnCertificate.Visible = earnsCertificate;

            rptAnswers.DataSource = result.Answers;
            rptAnswers.DataBind();

            string script = $@"
                var chartElement = document.getElementById('resultChart');
                if (chartElement) {{
                    var ctx = chartElement.getContext('2d');
                    new Chart(ctx, {{
                        type: 'doughnut',
                        data: {{
                            labels: ['Correct', 'Incorrect'],
                            datasets: [{{
                                data: [{result.CorrectAnswers}, {Math.Max(0, result.TotalQuestions - result.CorrectAnswers)}],
                                backgroundColor: ['#48bb78', '#fc8181'],
                                borderWidth: 0
                            }}]
                        }},
                        options: {{
                            responsive: true,
                            maintainAspectRatio: true,
                            plugins: {{ legend: {{ position: 'bottom' }} }}
                        }}
                    }});
                }}";

            ClientScript.RegisterStartupScript(this.GetType(), "chart", script, true);
        }

        protected void btnRetake_Click(object sender, EventArgs e)
        {
            int quizId;
            if (int.TryParse(Request.QueryString["id"], out quizId))
                Response.Redirect($"~/Learner/TakeQuiz.aspx?id={quizId}", false);
            else
                Response.Redirect("~/Learner/Quizzes.aspx", false);

            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnCertificate_Click(object sender, EventArgs e)
        {
            int quizId;
            var learnerId = SessionHelper.GetCurrentUserId();
            var result = int.TryParse(Request.QueryString["id"], out quizId) && learnerId.HasValue
                ? Session[TakeQuiz.GetResultSessionKey(learnerId.Value, quizId)] as QuizResultSnapshot
                : null;

            if (!HasCompleteAnswers(result) || result.QuizID != quizId || result.LearnerID != learnerId.GetValueOrDefault() ||
                result.PercentageScore <= CertificateMinimumScoreExclusive ||
                !QuizAccessHelper.IsModuleCompleted(result.ModuleID, learnerId.GetValueOrDefault()))
            {
                Response.Redirect("~/Learner/Quizzes.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            var certificate = CertificateHelper.GetEligibleCertificateForModule(
                learnerId.Value, result.ModuleID, CertificateMinimumScoreExclusive);

            if (certificate == null)
            {
                var user = SessionHelper.GetCurrentUser();
                if (user == null)
                {
                    ShowCertificateError();
                    return;
                }

                certificate = new Certificate
                {
                    LearnerID = learnerId.Value,
                    ModuleID = result.ModuleID,
                    LearnerName = user.FullName,
                    ModuleTitle = result.ModuleTitle,
                    Score = result.PercentageScore,
                    IsActive = true,
                    Issuer = "RespondX Training Institute"
                };

                if (string.IsNullOrEmpty(CertificateHelper.GenerateCertificate(certificate)) ||
                    !CertificateHelper.SaveCertificate(certificate))
                {
                    ShowCertificateError();
                    return;
                }

                certificate = CertificateHelper.GetEligibleCertificateForModule(
                    learnerId.Value, result.ModuleID, CertificateMinimumScoreExclusive);
            }

            if (certificate == null)
            {
                ShowCertificateError();
                return;
            }

            Response.Redirect($"~/Certificates/Download.aspx?id={certificate.CertificateID}", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowCertificateError()
        {
            ClientScript.RegisterStartupScript(this.GetType(), "certificate-error",
                "showNotification('Unable to prepare your certificate. Please try again later.', 'error');", true);
        }

        private static bool HasCompleteAnswers(QuizResultSnapshot result)
        {
            return result != null && result.CompletedAllQuestions && result.TotalQuestions > 0 &&
                result.Answers != null && result.Answers.Count == result.TotalQuestions;
        }
    }
}
