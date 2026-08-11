using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageQuestions : Page
    {
        private int quizId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                if (int.TryParse(Request.QueryString["quizId"], out quizId))
                {
                    hfQuizID.Value = quizId.ToString();
                    LoadQuizInfo();
                    LoadQuestions();
                }
                else
                {
                    Response.Redirect("ManageQuizzes.aspx");
                }
            }
            else
            {
                quizId = int.Parse(hfQuizID.Value);
            }
        }

        private void LoadQuizInfo()
        {
            var quiz = GetQuizById(quizId);
            if (quiz != null)
            {
                lblQuizInfo.Text = $"Quiz: {quiz.Title}";
            }
        }

        private void LoadQuestions()
        {
            var questions = GetQuestions(quizId);
            rptQuestions.DataSource = questions;
            rptQuestions.DataBind();
        }

        private QuizItem GetQuizById(int id)
        {
            var quizzes = new List<QuizItem>
            {
                new QuizItem { QuizID = 1, Title = "Emergency Response Fundamentals" },
                new QuizItem { QuizID = 2, Title = "CPR Certification Quiz" }
            };
            return quizzes.Find(q => q.QuizID == id);
        }

        private List<QuestionItem> GetQuestions(int quizId)
        {
            return new List<QuestionItem>
            {
                new QuestionItem
                {
                    QuestionID = 1,
                    QuestionText = "What is the first step in emergency response?",
                    QuestionType = "MultipleChoice",
                    QuestionTypeDisplay = "Multiple Choice",
                    Points = 5,
                    IsActive = true,
                    Options = new List<OptionItem>
                    {
                        new OptionItem { OptionID = 1, OptionText = "Call 911", OptionLabel = "A", IsCorrect = true },
                        new OptionItem { OptionID = 2, OptionText = "Run away", OptionLabel = "B", IsCorrect = false },
                        new OptionItem { OptionID = 3, OptionText = "Wait for help", OptionLabel = "C", IsCorrect = false }
                    }
                }
            };
        }

        protected void rptQuestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int questionId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Edit":
                    EditQuestion(questionId);
                    break;
                case "Toggle":
                    ToggleQuestion(questionId);
                    break;
                case "Delete":
                    DeleteQuestion(questionId);
                    break;
            }
        }

        private void EditQuestion(int questionId)
        {
            var question = GetQuestionById(questionId);
            if (question != null)
            {
                lblModalTitle.Text = "Edit Question";
                hfQuestionID.Value = questionId.ToString();
                txtQuestionText.Text = question.QuestionText;
                ddlQuestionType.SelectedValue = question.QuestionType;
                txtPoints.Text = question.Points.ToString();
                chkIsActive.Checked = question.IsActive;

                var options = question.Options;
                if (options.Count >= 1) { txtOption1.Text = options[0].OptionText; chkCorrect1.Checked = options[0].IsCorrect; }
                if (options.Count >= 2) { txtOption2.Text = options[1].OptionText; chkCorrect2.Checked = options[1].IsCorrect; }
                if (options.Count >= 3) { txtOption3.Text = options[2].OptionText; chkCorrect3.Checked = options[2].IsCorrect; }

                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalQuestion').modal('show');", true);
            }
        }

        private void ToggleQuestion(int questionId)
        {
            ShowSuccess("Question status updated successfully");
            LoadQuestions();
        }

        private void DeleteQuestion(int questionId)
        {
            ShowSuccess("Question deleted successfully");
            LoadQuestions();
        }

        protected void btnSaveQuestion_Click(object sender, EventArgs e)
        {
            int questionId;
            bool isNew = !int.TryParse(hfQuestionID.Value, out questionId) || questionId == 0;

            if (string.IsNullOrEmpty(txtQuestionText.Text))
            {
                ShowError("Question text is required.");
                return;
            }

            ShowSuccess(isNew ? "Question added successfully" : "Question updated successfully");
            ClearForm();
            LoadQuestions();

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalQuestion').modal('hide');", true);
        }

        private QuestionItem GetQuestionById(int questionId)
        {
            var questions = GetQuestions(quizId);
            return questions.Find(q => q.QuestionID == questionId);
        }

        private void ClearForm()
        {
            hfQuestionID.Value = "";
            txtQuestionText.Text = "";
            ddlQuestionType.SelectedIndex = 0;
            txtPoints.Text = "5";
            txtOption1.Text = "";
            txtOption2.Text = "";
            txtOption3.Text = "";
            txtOption4.Text = "";
            chkCorrect1.Checked = false;
            chkCorrect2.Checked = false;
            chkCorrect3.Checked = false;
            chkCorrect4.Checked = false;
            chkIsActive.Checked = true;
            lblModalTitle.Text = "Add Question";
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalQuestion').modal('show');", true);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ManageQuizzes.aspx?moduleId={Request.QueryString["moduleId"]}");
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            lblSuccess.Text = message;
            pnlError.Visible = false;
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
            pnlSuccess.Visible = false;
        }
    }
}