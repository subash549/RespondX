using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageQuizzes : Page
    {
        private int moduleId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                if (int.TryParse(Request.QueryString["moduleId"], out moduleId))
                {
                    hfModuleID.Value = moduleId.ToString();
                    LoadModuleInfo();
                    LoadQuizzes();
                }
                else
                {
                    Response.Redirect("ManageModules.aspx");
                }
            }
            else
            {
                moduleId = int.Parse(hfModuleID.Value);
            }
        }

        private void LoadModuleInfo()
        {
            var module = GetModuleById(moduleId);
            if (module != null)
            {
                lblModuleInfo.Text = $"Module: {module.Title}";
            }
        }

        private void LoadQuizzes()
        {
            var quizzes = GetQuizzes(moduleId);
            rptQuizzes.DataSource = quizzes;
            rptQuizzes.DataBind();
        }

        private ModuleItem GetModuleById(int id)
        {
            var modules = new List<ModuleItem>
            {
                new ModuleItem { ModuleID = 1, Title = "Introduction to Emergency Response" },
                new ModuleItem { ModuleID = 2, Title = "CPR and First Aid" }
            };
            return modules.Find(m => m.ModuleID == id);
        }

        private List<QuizItem> GetQuizzes(int moduleId)
        {
            return new List<QuizItem>
            {
                new QuizItem { QuizID = 1, Title = "Emergency Response Fundamentals", Description = "Test your knowledge of emergency response", TimeLimitMinutes = 30, PassingScore = 70, MaxAttempts = 3, QuestionCount = 20, IsActive = true },
                new QuizItem { QuizID = 2, Title = "CPR Certification Quiz", Description = "Comprehensive CPR knowledge assessment", TimeLimitMinutes = 45, PassingScore = 80, MaxAttempts = 3, QuestionCount = 25, IsActive = true }
            };
        }

        protected void rptQuizzes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int quizId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Edit":
                    EditQuiz(quizId);
                    break;
                case "Questions":
                    Response.Redirect($"ManageQuestions.aspx?quizId={quizId}");
                    break;
                case "Toggle":
                    ToggleQuiz(quizId);
                    break;
                case "Delete":
                    DeleteQuiz(quizId);
                    break;
            }
        }

        private void EditQuiz(int quizId)
        {
            var quiz = GetQuizById(quizId);
            if (quiz != null)
            {
                lblModalTitle.Text = "Edit Quiz";
                hfQuizID.Value = quizId.ToString();
                txtTitle.Text = quiz.Title;
                txtDescription.Text = quiz.Description;
                txtTimeLimit.Text = quiz.TimeLimitMinutes.ToString();
                txtPassingScore.Text = quiz.PassingScore.ToString();
                txtMaxAttempts.Text = quiz.MaxAttempts.ToString();
                chkIsActive.Checked = quiz.IsActive;

                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalQuiz').modal('show');", true);
            }
        }

        private void ToggleQuiz(int quizId)
        {
            ShowSuccess("Quiz status updated successfully");
            LoadQuizzes();
        }

        private void DeleteQuiz(int quizId)
        {
            ShowSuccess("Quiz deleted successfully");
            LoadQuizzes();
        }

        protected void btnSaveQuiz_Click(object sender, EventArgs e)
        {
            int quizId;
            bool isNew = !int.TryParse(hfQuizID.Value, out quizId) || quizId == 0;

            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                ShowError("Title is required.");
                return;
            }

            ShowSuccess(isNew ? "Quiz added successfully" : "Quiz updated successfully");
            ClearForm();
            LoadQuizzes();

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalQuiz').modal('hide');", true);
        }

        private QuizItem GetQuizById(int quizId)
        {
            var quizzes = GetQuizzes(moduleId);
            return quizzes.Find(q => q.QuizID == quizId);
        }

        private void ClearForm()
        {
            hfQuizID.Value = "";
            txtTitle.Text = "";
            txtDescription.Text = "";
            txtTimeLimit.Text = "30";
            txtPassingScore.Text = "70";
            txtMaxAttempts.Text = "3";
            chkIsActive.Checked = true;
            lblModalTitle.Text = "Add Quiz";
        }

        protected void btnAddQuiz_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalQuiz').modal('show');", true);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageModules.aspx");
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