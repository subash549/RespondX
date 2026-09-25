using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageScenarios : Page
    {
        private static string ConnectionString => DatabaseHelper.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                LoadModuleLists();

                int moduleId;
                if (int.TryParse(Request.QueryString["moduleId"], out moduleId) &&
                    ddlModuleFilter.Items.FindByValue(moduleId.ToString()) != null)
                {
                    ddlModuleFilter.SelectedValue = moduleId.ToString();
                }

                LoadScenarios();
            }
        }

        private void LoadModuleLists()
        {
            ddlModuleFilter.Items.Clear();
            ddlModuleFilter.Items.Add(new ListItem("All modules", ""));
            ddlScenarioModule.Items.Clear();
            ddlScenarioModule.Items.Add(new ListItem("-- Select Module --", ""));

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("SELECT ModuleID, Title, IsActive FROM dbo.Modules ORDER BY ModuleOrder, Title;", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader["ModuleID"].ToString();
                        string title = reader["Title"].ToString() + (Convert.ToBoolean(reader["IsActive"]) ? "" : " (inactive)");
                        ddlModuleFilter.Items.Add(new ListItem(title, id));
                        ddlScenarioModule.Items.Add(new ListItem(title, id));
                    }
                }
            }
        }

        private int? SelectedModuleFilter
        {
            get
            {
                int id;
                return int.TryParse(ddlModuleFilter.SelectedValue, out id) ? id : (int?)null;
            }
        }

        private void LoadScenarios()
        {
            var scenarios = new List<ScenarioItem>();
            int difficulty;
            bool filterDifficulty = int.TryParse(ddlDifficulty.SelectedValue, out difficulty);

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT s.ScenarioID, s.Title, s.Description, s.ScenarioOrder, s.DifficultyLevel, s.IsActive,
                       m.Title AS ModuleTitle,
                       (SELECT COUNT(*) FROM dbo.ScenarioOptions o WHERE o.ScenarioID = s.ScenarioID) AS OptionsCount
                FROM dbo.Scenarios s
                INNER JOIN dbo.Modules m ON m.ModuleID = s.ModuleID
                WHERE (@ModuleID IS NULL OR s.ModuleID = @ModuleID)
                  AND (@Difficulty IS NULL OR s.DifficultyLevel = @Difficulty)
                ORDER BY m.ModuleOrder, s.ScenarioOrder, s.ScenarioID;", conn))
            {
                var moduleId = SelectedModuleFilter;
                cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId.HasValue ? (object)moduleId.Value : DBNull.Value;
                cmd.Parameters.Add("@Difficulty", SqlDbType.Int).Value = filterDifficulty ? (object)difficulty : DBNull.Value;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int level = Convert.ToInt32(reader["DifficultyLevel"]);
                        scenarios.Add(new ScenarioItem
                        {
                            ScenarioID = Convert.ToInt32(reader["ScenarioID"]),
                            Title = Convert.ToString(reader["Title"]),
                            Description = Convert.ToString(reader["Description"]),
                            ScenarioOrder = Convert.ToInt32(reader["ScenarioOrder"]),
                            DifficultyLevel = level,
                            DifficultyDisplay = UiHelper.DifficultyLabel(level),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            Module = Convert.ToString(reader["ModuleTitle"]),
                            OptionsCount = Convert.ToInt32(reader["OptionsCount"])
                        });
                    }
                }
            }

            rptScenarios.DataSource = scenarios;
            rptScenarios.DataBind();
            pnlNoScenarios.Visible = scenarios.Count == 0;
        }

        protected void rptScenarios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int scenarioId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out scenarioId))
                return;

            switch (e.CommandName)
            {
                case "EditScenario":
                    EditScenario(scenarioId);
                    break;
                case "Options":
                    OpenOptions(scenarioId);
                    break;
                case "ToggleScenario":
                    ToggleScenario(scenarioId);
                    break;
                case "DeleteScenario":
                    DeleteScenario(scenarioId);
                    break;
            }
        }

        private void EditScenario(int scenarioId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT ModuleID, Title, Description, ScenarioText, ScenarioOrder, DifficultyLevel, IsActive
                FROM dbo.Scenarios WHERE ScenarioID = @ID;", conn))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = scenarioId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        ShowError("That scenario no longer exists.");
                        LoadScenarios();
                        return;
                    }

                    ClearForm();
                    lblModalTitle.Text = "Edit Scenario";
                    hfScenarioID.Value = scenarioId.ToString();
                    SelectIfPresent(ddlScenarioModule, reader["ModuleID"].ToString());
                    txtTitle.Text = Convert.ToString(reader["Title"]);
                    txtDescription.Text = Convert.ToString(reader["Description"]);
                    txtScenarioText.Text = Convert.ToString(reader["ScenarioText"]);
                    txtOrder.Text = reader["ScenarioOrder"].ToString();
                    SelectIfPresent(ddlDifficultyLevel, reader["DifficultyLevel"].ToString());
                    chkIsActive.Checked = Convert.ToBoolean(reader["IsActive"]);
                }
            }

            UiHelper.ShowModal(this, "modalScenario");
        }

        private static void SelectIfPresent(DropDownList list, string value)
        {
            if (list.Items.FindByValue(value) != null)
                list.SelectedValue = value;
        }

        private void ToggleScenario(int scenarioId)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(@"
                    UPDATE dbo.Scenarios
                    SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END, UpdatedAt = GETDATE()
                    WHERE ScenarioID = @ID;", conn))
                {
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = scenarioId;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                ShowSuccess("Scenario status updated.");
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Toggle scenario", ex.Message, ex.StackTrace);
                ShowError("Unable to update this scenario. Please try again.");
            }
            LoadScenarios();
        }

        private void DeleteScenario(int scenarioId)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    using (var cmd = new SqlCommand(@"
                        UPDATE dbo.LearnerProgress SET ScenarioID = NULL WHERE ScenarioID = @ID;
                        DELETE FROM dbo.ScenarioOptions WHERE ScenarioID = @ID;
                        DELETE FROM dbo.Scenarios WHERE ScenarioID = @ID;", conn, transaction))
                    {
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = scenarioId;
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
                ShowSuccess("Scenario deleted.");
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Delete scenario", ex.Message, ex.StackTrace);
                ShowError("Unable to delete this scenario. Please try again.");
            }
            LoadScenarios();
        }

        protected void btnSaveScenario_Click(object sender, EventArgs e)
        {
            int scenarioId;
            bool isNew = !int.TryParse(hfScenarioID.Value, out scenarioId) || scenarioId <= 0;
            int moduleId, difficulty, order = 0;
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            string scenarioText = txtScenarioText.Text.Trim();

            if (!int.TryParse(ddlScenarioModule.SelectedValue, out moduleId))
            {
                ShowFormError("Choose the module this scenario belongs to.");
                return;
            }
            if (title.Length == 0 || title.Length > 100 || scenarioText.Length == 0)
            {
                ShowFormError("Enter a title (up to 100 characters) and the scenario text.");
                return;
            }
            if (description.Length > 500)
            {
                ShowFormError("The short description must be 500 characters or fewer.");
                return;
            }
            if (txtOrder.Text.Trim().Length > 0 && (!int.TryParse(txtOrder.Text.Trim(), out order) || order < 1))
            {
                ShowFormError("Scenario order must be a whole number of 1 or higher.");
                return;
            }
            if (!int.TryParse(ddlDifficultyLevel.SelectedValue, out difficulty) || difficulty < 1 || difficulty > 5)
                difficulty = 1;

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(isNew
                    ? @"INSERT INTO dbo.Scenarios (ModuleID, Title, Description, ScenarioText, ScenarioOrder, DifficultyLevel, IsActive)
                        VALUES (@ModuleID, @Title, @Description, @ScenarioText,
                                CASE WHEN @Order > 0 THEN @Order
                                     ELSE (SELECT ISNULL(MAX(ScenarioOrder), 0) + 1 FROM dbo.Scenarios WHERE ModuleID = @ModuleID) END,
                                @Difficulty, @IsActive);"
                    : @"UPDATE dbo.Scenarios
                        SET ModuleID = @ModuleID, Title = @Title, Description = @Description, ScenarioText = @ScenarioText,
                            ScenarioOrder = CASE WHEN @Order > 0 THEN @Order ELSE ScenarioOrder END,
                            DifficultyLevel = @Difficulty, IsActive = @IsActive, UpdatedAt = GETDATE()
                        WHERE ScenarioID = @ID;", conn))
                {
                    cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId;
                    cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = title;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = description.Length == 0 ? (object)DBNull.Value : description;
                    cmd.Parameters.Add("@ScenarioText", SqlDbType.NVarChar, -1).Value = scenarioText;
                    cmd.Parameters.Add("@Order", SqlDbType.Int).Value = order;
                    cmd.Parameters.Add("@Difficulty", SqlDbType.Int).Value = difficulty;
                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = chkIsActive.Checked;
                    if (!isNew)
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = scenarioId;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Save scenario", ex.Message, ex.StackTrace);
                ShowFormError("Unable to save this scenario. Please try again.");
                return;
            }

            ShowSuccess(isNew ? "Scenario added. Use Options to add its answers." : "Scenario updated.");
            ClearForm();
            LoadScenarios();
            UiHelper.HideModal(this, "modalScenario");
        }

        // ========== Options ==========

        private void OpenOptions(int scenarioId)
        {
            hfOptionsScenarioID.Value = scenarioId.ToString();
            ClearOptionForm();
            pnlOptionError.Visible = false;
            LoadOptions(scenarioId);
            UiHelper.ShowModal(this, "modalOptions");
        }

        private void LoadOptions(int scenarioId)
        {
            var options = new List<ScenarioOptionItem>();
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT Title FROM dbo.Scenarios WHERE ScenarioID = @ID;
                SELECT OptionID, OptionText, IsCorrect, Feedback, OptionOrder
                FROM dbo.ScenarioOptions WHERE ScenarioID = @ID
                ORDER BY OptionOrder, OptionID;", conn))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = scenarioId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    lblOptionsScenario.Text = reader.Read() ? Server.HtmlEncode(Convert.ToString(reader["Title"])) : string.Empty;
                    reader.NextResult();
                    while (reader.Read())
                    {
                        options.Add(new ScenarioOptionItem
                        {
                            OptionID = Convert.ToInt32(reader["OptionID"]),
                            OptionText = Convert.ToString(reader["OptionText"]),
                            IsCorrect = Convert.ToBoolean(reader["IsCorrect"]),
                            Feedback = Convert.ToString(reader["Feedback"]),
                            OptionOrder = Convert.ToInt32(reader["OptionOrder"])
                        });
                    }
                }
            }

            rptOptions.DataSource = options;
            rptOptions.DataBind();
            rptOptions.Visible = options.Count > 0;
            lblNoOptions.Visible = options.Count == 0;
        }

        private int OptionsScenarioId
        {
            get
            {
                int id;
                return int.TryParse(hfOptionsScenarioID.Value, out id) ? id : 0;
            }
        }

        protected void rptOptions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int optionId;
            int scenarioId = OptionsScenarioId;
            if (scenarioId <= 0 || !int.TryParse(Convert.ToString(e.CommandArgument), out optionId))
                return;

            pnlOptionError.Visible = false;
            if (e.CommandName == "EditOption")
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(@"
                    SELECT OptionText, Feedback, IsCorrect FROM dbo.ScenarioOptions
                    WHERE OptionID = @OptionID AND ScenarioID = @ScenarioID;", conn))
                {
                    cmd.Parameters.Add("@OptionID", SqlDbType.Int).Value = optionId;
                    cmd.Parameters.Add("@ScenarioID", SqlDbType.Int).Value = scenarioId;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            hfOptionID.Value = optionId.ToString();
                            txtOptionText.Text = Convert.ToString(reader["OptionText"]);
                            txtOptionFeedback.Text = Convert.ToString(reader["Feedback"]);
                            chkIsCorrectOption.Checked = Convert.ToBoolean(reader["IsCorrect"]);
                            lblOptionFormTitle.Text = "Edit option";
                            btnSaveOption.Text = "Update Option";
                            btnCancelOptionEdit.Visible = true;
                        }
                    }
                }
            }
            else if (e.CommandName == "DeleteOption")
            {
                try
                {
                    using (var conn = new SqlConnection(ConnectionString))
                    using (var cmd = new SqlCommand(@"
                        DELETE FROM dbo.ScenarioOptions WHERE OptionID = @OptionID AND ScenarioID = @ScenarioID;
                        -- Keep option numbering continuous (1, 2, 3...).
                        WITH ordered AS (
                            SELECT OptionOrder, ROW_NUMBER() OVER (ORDER BY OptionOrder, OptionID) AS NewOrder
                            FROM dbo.ScenarioOptions WHERE ScenarioID = @ScenarioID)
                        UPDATE ordered SET OptionOrder = NewOrder;", conn))
                    {
                        cmd.Parameters.Add("@OptionID", SqlDbType.Int).Value = optionId;
                        cmd.Parameters.Add("@ScenarioID", SqlDbType.Int).Value = scenarioId;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    ClearOptionForm();
                    UiHelper.Notify(this, "Option deleted.");
                }
                catch (Exception ex)
                {
                    DatabaseHelper.LogError("Delete scenario option", ex.Message, ex.StackTrace);
                    ShowOptionError("Unable to delete this option. Please try again.");
                }
                LoadScenarios();
            }

            LoadOptions(scenarioId);
            UiHelper.ShowModal(this, "modalOptions");
        }

        protected void btnSaveOption_Click(object sender, EventArgs e)
        {
            int scenarioId = OptionsScenarioId;
            string text = txtOptionText.Text.Trim();
            string feedback = txtOptionFeedback.Text.Trim();
            int optionId;
            bool isNew = !int.TryParse(hfOptionID.Value, out optionId) || optionId <= 0;

            if (scenarioId <= 0)
                return;

            if (text.Length == 0)
            {
                ShowOptionError("Enter the option text.");
            }
            else if (feedback.Length > 500)
            {
                ShowOptionError("Feedback must be 500 characters or fewer.");
            }
            else
            {
                try
                {
                    using (var conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        using (var transaction = conn.BeginTransaction())
                        {
                            // Scenarios have a single best answer, so marking one option correct clears the others.
                            if (chkIsCorrectOption.Checked)
                            {
                                using (var clear = new SqlCommand(
                                    "UPDATE dbo.ScenarioOptions SET IsCorrect = 0 WHERE ScenarioID = @ScenarioID;", conn, transaction))
                                {
                                    clear.Parameters.Add("@ScenarioID", SqlDbType.Int).Value = scenarioId;
                                    clear.ExecuteNonQuery();
                                }
                            }

                            using (var cmd = new SqlCommand(isNew
                                ? @"INSERT INTO dbo.ScenarioOptions (ScenarioID, OptionText, IsCorrect, Feedback, OptionOrder)
                                    VALUES (@ScenarioID, @Text, @IsCorrect, @Feedback,
                                            (SELECT ISNULL(MAX(OptionOrder), 0) + 1 FROM dbo.ScenarioOptions WHERE ScenarioID = @ScenarioID));"
                                : @"UPDATE dbo.ScenarioOptions
                                    SET OptionText = @Text, IsCorrect = @IsCorrect, Feedback = @Feedback
                                    WHERE OptionID = @OptionID AND ScenarioID = @ScenarioID;", conn, transaction))
                            {
                                cmd.Parameters.Add("@ScenarioID", SqlDbType.Int).Value = scenarioId;
                                cmd.Parameters.Add("@Text", SqlDbType.NVarChar, -1).Value = text;
                                cmd.Parameters.Add("@IsCorrect", SqlDbType.Bit).Value = chkIsCorrectOption.Checked;
                                cmd.Parameters.Add("@Feedback", SqlDbType.NVarChar, 500).Value = feedback.Length == 0 ? (object)DBNull.Value : feedback;
                                if (!isNew)
                                    cmd.Parameters.Add("@OptionID", SqlDbType.Int).Value = optionId;
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                    }

                    ClearOptionForm();
                    pnlOptionError.Visible = false;
                    UiHelper.Notify(this, isNew ? "Option added." : "Option updated.");
                    LoadScenarios();
                }
                catch (Exception ex)
                {
                    DatabaseHelper.LogError("Save scenario option", ex.Message, ex.StackTrace);
                    ShowOptionError("Unable to save this option. Please try again.");
                }
            }

            LoadOptions(scenarioId);
            UiHelper.ShowModal(this, "modalOptions");
        }

        protected void btnCancelOptionEdit_Click(object sender, EventArgs e)
        {
            ClearOptionForm();
            pnlOptionError.Visible = false;
            if (OptionsScenarioId > 0)
                LoadOptions(OptionsScenarioId);
            UiHelper.ShowModal(this, "modalOptions");
        }

        private void ClearOptionForm()
        {
            hfOptionID.Value = string.Empty;
            txtOptionText.Text = string.Empty;
            txtOptionFeedback.Text = string.Empty;
            chkIsCorrectOption.Checked = false;
            lblOptionFormTitle.Text = "Add an option";
            btnSaveOption.Text = "Add Option";
            btnCancelOptionEdit.Visible = false;
        }

        private void ShowOptionError(string message)
        {
            pnlOptionError.Visible = true;
            lblOptionError.Text = Server.HtmlEncode(message);
        }

        // ========== Shared ==========

        private void ClearForm()
        {
            hfScenarioID.Value = string.Empty;
            txtTitle.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtScenarioText.Text = string.Empty;
            txtOrder.Text = string.Empty;
            ddlDifficultyLevel.SelectedIndex = 0;
            chkIsActive.Checked = true;
            lblModalTitle.Text = "Add Scenario";
            pnlModalError.Visible = false;

            var moduleId = SelectedModuleFilter;
            if (moduleId.HasValue)
                SelectIfPresent(ddlScenarioModule, moduleId.Value.ToString());
            else
                ddlScenarioModule.SelectedIndex = 0;
        }

        protected void btnAddScenario_Click(object sender, EventArgs e)
        {
            ClearForm();
            UiHelper.ShowModal(this, "modalScenario");
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            LoadScenarios();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageModules.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowFormError(string message)
        {
            pnlModalError.Visible = true;
            lblModalError.Text = Server.HtmlEncode(message);
            UiHelper.ShowModal(this, "modalScenario");
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
