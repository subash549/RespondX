using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageQuizzes : Page
    {
        private int moduleId;

        private static string ConnectionString => DatabaseHelper.ConnectionString;

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
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(
                "SELECT ModuleID, Title FROM dbo.Modules WHERE ModuleID = @ModuleID AND IsActive = 1;", connection))
            {
                command.Parameters.Add("@ModuleID", SqlDbType.Int).Value = id;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    return reader.Read()
                        ? new ModuleItem { ModuleID = Convert.ToInt32(reader["ModuleID"]), Title = Convert.ToString(reader["Title"]) }
                        : null;
                }
            }
        }

        private List<QuizItem> GetQuizzes(int moduleId)
        {
            var quizzes = new List<QuizItem>();
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(@"
                SELECT q.QuizID, q.Title, q.Description, q.TimeLimitMinutes, q.PassingScore,
                       q.MaxAttempts, q.IsActive,
                       (SELECT COUNT(*) FROM dbo.Questions question
                        WHERE question.QuizID = q.QuizID
                          AND question.QuestionType = N'MultipleChoice') AS QuestionCount
                FROM dbo.Quizzes q
                WHERE q.ModuleID = @ModuleID
                ORDER BY q.CreatedAt DESC, q.QuizID DESC;", connection))
            {
                command.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        quizzes.Add(new QuizItem
                        {
                            QuizID = Convert.ToInt32(reader["QuizID"]),
                            Title = Convert.ToString(reader["Title"]),
                            Description = reader["Description"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Description"]),
                            TimeLimitMinutes = Convert.ToInt32(reader["TimeLimitMinutes"]),
                            PassingScore = Convert.ToInt32(reader["PassingScore"]),
                            MaxAttempts = Convert.ToInt32(reader["MaxAttempts"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            QuestionCount = Convert.ToInt32(reader["QuestionCount"])
                        });
                    }
                }
            }
            return quizzes;
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
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(@"
                    UPDATE dbo.Quizzes
                    SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END, UpdatedAt = GETDATE()
                    WHERE QuizID = @QuizID AND ModuleID = @ModuleID;", connection))
                {
                    command.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizId;
                    command.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId;
                    connection.Open();
                    if (command.ExecuteNonQuery() != 1)
                    {
                        ShowError("That quiz could not be found in this module.");
                        return;
                    }
                }
                ShowSuccess("Quiz status updated successfully.");
                LoadQuizzes();
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Toggle quiz status", ex.Message, ex.StackTrace);
                ShowError("The quiz status could not be updated.");
            }
        }

        private void DeleteQuiz(int quizId)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(@"
                    UPDATE dbo.Quizzes
                    SET IsActive = 0, UpdatedAt = GETDATE()
                    WHERE QuizID = @QuizID AND ModuleID = @ModuleID;", connection))
                {
                    command.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizId;
                    command.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId;
                    connection.Open();
                    if (command.ExecuteNonQuery() != 1)
                    {
                        ShowError("That quiz could not be found in this module.");
                        return;
                    }
                }
                ShowSuccess("Quiz deactivated. Existing learner results have been preserved.");
                LoadQuizzes();
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Deactivate quiz", ex.Message, ex.StackTrace);
                ShowError("The quiz could not be deactivated.");
            }
        }

        protected void btnSaveQuiz_Click(object sender, EventArgs e)
        {
            int quizId;
            bool isNew = !int.TryParse(hfQuizID.Value, out quizId) || quizId == 0;
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            int timeLimit;
            int passingScore;
            int maxAttempts;

            if (string.IsNullOrWhiteSpace(title) || title.Length > 100)
            {
                ShowError("Enter a quiz title with no more than 100 characters.");
                return;
            }
            if (description.Length > 500)
            {
                ShowError("The description must be 500 characters or fewer.");
                return;
            }
            if (!int.TryParse(txtTimeLimit.Text, out timeLimit) || timeLimit < 1 || timeLimit > 180)
            {
                ShowError("Time limit must be between 1 and 180 minutes.");
                return;
            }
            if (!int.TryParse(txtPassingScore.Text, out passingScore) || passingScore < 0 || passingScore > 100)
            {
                ShowError("Passing score must be between 0 and 100.");
                return;
            }
            if (!int.TryParse(txtMaxAttempts.Text, out maxAttempts) || maxAttempts < 1 || maxAttempts > 10)
            {
                ShowError("Maximum attempts must be between 1 and 10.");
                return;
            }

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(isNew ? @"
                    INSERT INTO dbo.Quizzes
                        (ModuleID, Title, Description, TimeLimitMinutes, PassingScore, MaxAttempts, IsActive)
                    VALUES
                        (@ModuleID, @Title, @Description, @TimeLimitMinutes, @PassingScore, @MaxAttempts, @IsActive);" : @"
                    UPDATE dbo.Quizzes
                    SET Title = @Title, Description = @Description, TimeLimitMinutes = @TimeLimitMinutes,
                        PassingScore = @PassingScore, MaxAttempts = @MaxAttempts, IsActive = @IsActive,
                        UpdatedAt = GETDATE()
                    WHERE QuizID = @QuizID AND ModuleID = @ModuleID;", connection))
                {
                    command.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId;
                    command.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = title;
                    command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                        string.IsNullOrWhiteSpace(description) ? (object)DBNull.Value : description;
                    command.Parameters.Add("@TimeLimitMinutes", SqlDbType.Int).Value = timeLimit;
                    command.Parameters.Add("@PassingScore", SqlDbType.Int).Value = passingScore;
                    command.Parameters.Add("@MaxAttempts", SqlDbType.Int).Value = maxAttempts;
                    command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = chkIsActive.Checked;
                    if (!isNew)
                        command.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizId;

                    connection.Open();
                    if (command.ExecuteNonQuery() != 1)
                    {
                        ShowError("That quiz could not be found in this module.");
                        return;
                    }
                }

                ShowSuccess(isNew ? "Quiz added successfully." : "Quiz updated successfully.");
                try { NotificationRepository.NotifyRoles("Content", isNew ? "New quiz available" : "Quiz updated", title, "~/Learner/Quizzes.aspx", SessionHelper.GetCurrentUserId(), "Learner", "Expert"); }
                catch (Exception notificationError) { DatabaseHelper.LogError("Quiz notification", notificationError.Message, notificationError.StackTrace); }
                ClearForm();
                LoadQuizzes();
                ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalQuiz').modal('hide');", true);
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Save quiz", ex.Message, ex.StackTrace);
                ShowError("The quiz could not be saved. Check the database connection and try again.");
            }
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
