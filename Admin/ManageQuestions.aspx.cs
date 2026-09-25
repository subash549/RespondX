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
    public partial class ManageQuestions : Page
    {
        private int quizId;

        private static string ConnectionString
        {
            get
            {
                var setting = ConfigurationManager.ConnectionStrings["DefaultConnection"]
                    ?? ConfigurationManager.ConnectionStrings["RespondX"];
                if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                    throw new InvalidOperationException("The RespondX database connection is not configured.");
                return setting.ConnectionString;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                EnsureEveryModuleHasAQuiz();
                BindQuizSelector();
                int requestedQuizId;
                if (!int.TryParse(Request.QueryString["quizId"], out requestedQuizId) || !QuizExists(requestedQuizId))
                    requestedQuizId = ddlQuiz.Items.Count == 0 ? 0 : Convert.ToInt32(ddlQuiz.Items[0].Value);

                if (requestedQuizId <= 0)
                {
                    hfQuizID.Value = string.Empty;
                    rptQuestions.DataSource = new List<QuestionItem>();
                    rptQuestions.DataBind();
                    ShowError("Create a quiz for a module before adding questions.");
                    return;
                }

                quizId = requestedQuizId;
                ddlQuiz.SelectedValue = quizId.ToString();
                hfQuizID.Value = quizId.ToString();
                LoadSelectedQuiz();
            }
            else if (!int.TryParse(hfQuizID.Value, out quizId) || !QuizExists(quizId))
            {
                quizId = 0;
                hfQuizID.Value = string.Empty;
            }
        }

        private void BindQuizSelector()
        {
            ddlQuiz.Items.Clear();
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(@"
                SELECT q.QuizID, q.Title, m.Title AS ModuleTitle
                FROM dbo.Quizzes q
                INNER JOIN dbo.Modules m ON m.ModuleID = q.ModuleID
                ORDER BY m.ModuleOrder, q.Title;", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ddlQuiz.Items.Add(new ListItem(
                            Convert.ToString(reader["Title"]) + " (" + Convert.ToString(reader["ModuleTitle"]) + ")",
                            Convert.ToString(reader["QuizID"])));
                    }
                }
            }
        }

        private void EnsureEveryModuleHasAQuiz()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        var modulesWithoutQuizzes = new List<ModuleItem>();
                        using (var command = new SqlCommand(@"
                            SELECT m.ModuleID, m.Title, m.Description
                            FROM dbo.Modules m WITH (UPDLOCK, HOLDLOCK)
                            WHERE NOT EXISTS
                            (
                                SELECT 1
                                FROM dbo.Quizzes q WITH (UPDLOCK, HOLDLOCK)
                                WHERE q.ModuleID = m.ModuleID
                            )
                            ORDER BY m.ModuleOrder, m.ModuleID;", connection, transaction))
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                modulesWithoutQuizzes.Add(new ModuleItem
                                {
                                    ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                    Title = Convert.ToString(reader["Title"]),
                                    Description = reader["Description"] == DBNull.Value
                                        ? string.Empty
                                        : Convert.ToString(reader["Description"])
                                });
                            }
                        }

                        foreach (var module in modulesWithoutQuizzes)
                        {
                            var definition = QuizCatalog.GetAll().Find(item =>
                                string.Equals(item.ModuleTitle.Trim(), module.Title.Trim(), StringComparison.OrdinalIgnoreCase));
                            string title = definition == null ? module.Title.Trim() + " Quiz" : definition.Title;
                            if (title.Length > 100)
                                title = title.Substring(0, 100);
                            string description = definition == null
                                ? module.Description
                                : definition.Description;
                            if (description != null && description.Length > 500)
                                description = description.Substring(0, 500);

                            using (var insert = new SqlCommand(@"
                                INSERT INTO dbo.Quizzes
                                    (ModuleID, Title, Description, TimeLimitMinutes, PassingScore, MaxAttempts, IsActive)
                                VALUES
                                    (@ModuleID, @Title, @Description, @TimeLimitMinutes, @PassingScore, @MaxAttempts, 1);",
                                connection, transaction))
                            {
                                insert.Parameters.Add("@ModuleID", SqlDbType.Int).Value = module.ModuleID;
                                insert.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = title;
                                insert.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                                    string.IsNullOrWhiteSpace(description) ? (object)DBNull.Value : description;
                                insert.Parameters.Add("@TimeLimitMinutes", SqlDbType.Int).Value =
                                    definition == null ? 30 : definition.TimeLimitMinutes;
                                insert.Parameters.Add("@PassingScore", SqlDbType.Int).Value =
                                    definition == null ? 70 : definition.PassingScore;
                                insert.Parameters.Add("@MaxAttempts", SqlDbType.Int).Value =
                                    definition == null ? 3 : definition.MaxAttempts;
                                insert.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void LoadSelectedQuiz()
        {
            string title = null;
            string moduleTitle = null;
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(@"
                SELECT q.Title, m.Title AS ModuleTitle
                FROM dbo.Quizzes q
                INNER JOIN dbo.Modules m ON m.ModuleID = q.ModuleID
                WHERE q.QuizID = @QuizID;", connection))
            {
                command.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizId;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        title = Convert.ToString(reader["Title"]);
                        moduleTitle = Convert.ToString(reader["ModuleTitle"]);
                    }
                }
            }

            if (title == null)
                return;

            var builtInQuiz = QuizCatalog.GetAll().Find(definition =>
                string.Equals(definition.ModuleTitle.Trim(), moduleTitle.Trim(), StringComparison.OrdinalIgnoreCase));
            int builtInCount = builtInQuiz == null ? 0 : QuizQuestionBank.GetQuestions(builtInQuiz.QuizID).Count;
            lblQuizInfo.Text = Server.HtmlEncode(title + " (" + moduleTitle + ") | " + builtInCount +
                " built-in questions, plus administrator-added questions.");
            LoadQuestions();
        }

        private bool QuizExists(int selectedQuizId)
        {
            if (selectedQuizId <= 0)
                return false;

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand("SELECT COUNT(*) FROM dbo.Quizzes WHERE QuizID = @QuizID;", connection))
            {
                command.Parameters.Add("@QuizID", SqlDbType.Int).Value = selectedQuizId;
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar()) == 1;
            }
        }

        private void LoadQuestions()
        {
            try
            {
                List<QuestionItem> questions = QuizQuestionRepository.GetQuestionsForAdmin(quizId);
                rptQuestions.DataSource = questions;
                rptQuestions.DataBind();
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Load admin quiz questions", ex.Message, ex.StackTrace);
                rptQuestions.DataSource = new List<QuestionItem>();
                rptQuestions.DataBind();
                ShowError("The saved questions could not be loaded. Check the database connection and confirm that the RespondX schema has been applied.");
            }
        }

        protected void ddlQuiz_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedQuizId;
            if (!int.TryParse(ddlQuiz.SelectedValue, out selectedQuizId) || !QuizExists(selectedQuizId))
            {
                ShowError("Select a valid quiz.");
                return;
            }

            quizId = selectedQuizId;
            hfQuizID.Value = quizId.ToString();
            ClearForm();
            LoadSelectedQuiz();
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            if (!TryGetSelectedQuizId())
            {
                ShowError("Select a valid quiz before adding a question.");
                return;
            }

            ClearForm();
            ShowQuestionModal();
        }

        protected void btnSaveQuestion_Click(object sender, EventArgs e)
        {
            if (!TryGetSelectedQuizId())
            {
                ShowError("Select a valid quiz before saving a question.");
                return;
            }

            int points;
            if (!int.TryParse(txtPoints.Text, out points))
            {
                ShowError("Enter a valid points value between 1 and 100.");
                ShowQuestionModal();
                return;
            }

            bool[] correctAnswers = { rdoCorrect1.Checked, rdoCorrect2.Checked, rdoCorrect3.Checked, rdoCorrect4.Checked };
            int correctOptionIndex = -1;
            int correctCount = 0;
            for (int index = 0; index < correctAnswers.Length; index++)
            {
                if (correctAnswers[index])
                {
                    correctOptionIndex = index;
                    correctCount++;
                }
            }

            if (correctCount != 1)
            {
                ShowError("Mark exactly one answer as correct.");
                ShowQuestionModal();
                return;
            }

            string[] options = { txtOption1.Text, txtOption2.Text, txtOption3.Text, txtOption4.Text };
            try
            {
                int questionId;
                bool isEdit = int.TryParse(hfQuestionID.Value, out questionId) && questionId > 0;
                if (isEdit)
                    QuizQuestionRepository.UpdateQuestion(quizId, questionId, txtQuestionText.Text, points, options, correctOptionIndex);
                else
                    QuizQuestionRepository.AddQuestion(quizId, txtQuestionText.Text, points, options, correctOptionIndex);

                ClearForm();
                LoadSelectedQuiz();
                ShowSuccess(isEdit ? "Question updated. Learners will see the updated version." : "Question saved. Learners will see it in this quiz.");
                ScriptManager.RegisterStartupScript(this, GetType(), "hideQuestionModal", "$('#modalQuestion').modal('hide');", true);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
                ShowQuestionModal();
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Save admin quiz question", ex.Message, ex.StackTrace);
                ShowError("The question could not be saved. Check the database connection and confirm that the RespondX schema has been applied.");
                ShowQuestionModal();
            }
        }

        protected void rptQuestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Edit")
                return;

            int questionId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out questionId))
            {
                ShowError("Select a valid saved question.");
                return;
            }

            try
            {
                QuestionItem question = QuizQuestionRepository.GetQuestionsForAdmin(quizId)
                    .Find(item => item.QuestionID == questionId);
                if (question == null || question.Options == null || question.Options.Count != 4)
                {
                    ShowError("That question could not be loaded for editing. The editor requires exactly four choices.");
                    return;
                }

                hfQuestionID.Value = question.QuestionID.ToString();
                txtQuestionText.Text = question.QuestionText;
                txtPoints.Text = question.Points.ToString();
                TextBox[] optionInputs = { txtOption1, txtOption2, txtOption3, txtOption4 };
                RadioButton[] correctInputs = { rdoCorrect1, rdoCorrect2, rdoCorrect3, rdoCorrect4 };
                for (int index = 0; index < question.Options.Count; index++)
                {
                    optionInputs[index].Text = question.Options[index].OptionText;
                    correctInputs[index].Checked = question.Options[index].IsCorrect;
                }

                lblModalTitle.Text = "Edit Multiple-Choice Question";
                ShowQuestionModal();
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Load quiz question for editing", ex.Message, ex.StackTrace);
                ShowError("The question could not be loaded for editing. Check the database connection and quiz schema.");
            }
        }

        private bool TryGetSelectedQuizId()
        {
            int selectedQuizId;
            if (!int.TryParse(hfQuizID.Value, out selectedQuizId) || !QuizExists(selectedQuizId))
                return false;

            quizId = selectedQuizId;
            return true;
        }

        private void ClearForm()
        {
            hfQuestionID.Value = string.Empty;
            txtQuestionText.Text = string.Empty;
            txtPoints.Text = "1";
            txtOption1.Text = string.Empty;
            txtOption2.Text = string.Empty;
            txtOption3.Text = string.Empty;
            txtOption4.Text = string.Empty;
            rdoCorrect1.Checked = false;
            rdoCorrect2.Checked = false;
            rdoCorrect3.Checked = false;
            rdoCorrect4.Checked = false;
            lblModalTitle.Text = "Add Multiple-Choice Question";
        }

        private void ShowQuestionModal()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showQuestionModal", "$('#modalQuestion').modal('show');", true);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AdminDashboard.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            lblSuccess.Text = Server.HtmlEncode(message);
            pnlError.Visible = false;
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = Server.HtmlEncode(message);
            pnlSuccess.Visible = false;
        }
    }
}
