using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Expert
{
    public partial class ManageAssignments : Page
    {
        private static string connString => DatabaseHelper.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireAnyRole("Expert", "Admin"))
                return;

            if (!IsPostBack)
            {
                LoadModules();
                LoadAssignments();
            }
        }

        private void LoadModules()
        {
            ddlModule.Items.Clear();
            ddlModule.Items.Add(new ListItem("-- Select Module --", ""));
            
            var user = SessionHelper.GetCurrentUser();

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var query = user.Role == "Admin" ? 
                            "SELECT ModuleID, Title FROM Modules WHERE IsActive = 1" : 
                            "SELECT ModuleID, Title FROM Modules WHERE InstructorID = @ExpertID AND IsActive = 1";
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (user.Role != "Admin")
                    {
                        cmd.Parameters.AddWithValue("@ExpertID", user.UserID);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ddlModule.Items.Add(new ListItem(reader["Title"].ToString(), reader["ModuleID"].ToString()));
                        }
                    }
                }
            }
        }

        private void LoadAssignments()
        {
            var assignments = new List<AssignmentItem>();
            var user = SessionHelper.GetCurrentUser();

            using (var conn = new SqlConnection(connString))
            {
                var query = @"
                    SELECT a.AssignmentID, a.ModuleID, a.Title, a.Description, a.DueDate, a.MaxScore, a.IsActive, m.Title AS ModuleTitle
                    FROM Assignments a
                    INNER JOIN Modules m ON a.ModuleID = m.ModuleID";
                
                if (user.Role != "Admin")
                {
                    query += " WHERE m.InstructorID = @ExpertID";
                }
                
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (user.Role != "Admin")
                    {
                        cmd.Parameters.AddWithValue("@ExpertID", user.UserID);
                    }
                    
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            assignments.Add(new AssignmentItem
                            {
                                AssignmentID = Convert.ToInt32(reader["AssignmentID"]),
                                ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                ModuleTitle = reader["ModuleTitle"].ToString(),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                DueDate = Convert.ToDateTime(reader["DueDate"]),
                                MaxScore = Convert.ToInt32(reader["MaxScore"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            });
                        }
                    }
                }
            }

            rptAssignments.DataSource = assignments;
            rptAssignments.DataBind();
        }

        protected void rptAssignments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int assignmentId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out assignmentId))
                return;

            if (e.CommandName == "Edit")
            {
                EditAssignment(assignmentId);
            }
            else if (e.CommandName == "Toggle")
            {
                ToggleAssignment(assignmentId);
            }
        }

        private void EditAssignment(int assignmentId)
        {
            using (var conn = new SqlConnection(connString))
            {
                using (var cmd = new SqlCommand("SELECT ModuleID, Title, Description, DueDate, MaxScore, IsActive FROM Assignments WHERE AssignmentID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", assignmentId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ClearForm();
                            lblModalTitle.Text = "Edit Assignment";
                            hfAssignmentID.Value = assignmentId.ToString();
                            string moduleValue = reader["ModuleID"].ToString();
                            if (ddlModule.Items.FindByValue(moduleValue) == null)
                                ddlModule.Items.Add(new ListItem("Module #" + moduleValue + " (inactive)", moduleValue));
                            ddlModule.SelectedValue = moduleValue;
                            txtTitle.Text = reader["Title"].ToString();
                            txtDescription.Text = reader["Description"].ToString();
                            txtDueDate.Text = Convert.ToDateTime(reader["DueDate"]).ToString("yyyy-MM-dd");
                            txtMaxScore.Text = reader["MaxScore"].ToString();
                            chkIsActive.Checked = Convert.ToBoolean(reader["IsActive"]);

                            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalAssignment').modal('show');", true);
                        }
                    }
                }
            }
        }

        private void ToggleAssignment(int assignmentId)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                using (var cmd = new SqlCommand("UPDATE Assignments SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE AssignmentID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", assignmentId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                ShowSuccess("Assignment status updated.");
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Toggle assignment", ex.Message, ex.StackTrace);
                ShowError("Unable to update this assignment. Please try again.");
            }
            LoadAssignments();
        }

        protected void btnSaveAssignment_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            int moduleId;
            int maxScore = 100;
            DateTime dueDate = DateTime.Today.AddDays(7);

            if (title.Length == 0 || title.Length > 200 || !int.TryParse(ddlModule.SelectedValue, out moduleId))
            {
                ShowFormError("Choose a module and enter a title of up to 200 characters.");
                return;
            }
            if (txtMaxScore.Text.Trim().Length > 0 &&
                (!int.TryParse(txtMaxScore.Text.Trim(), out maxScore) || maxScore < 1 || maxScore > 1000))
            {
                ShowFormError("Max score must be a whole number between 1 and 1000.");
                return;
            }
            if (txtDueDate.Text.Trim().Length > 0 && !DateTime.TryParse(txtDueDate.Text.Trim(), out dueDate))
            {
                ShowFormError("Enter a valid due date.");
                return;
            }

            int assignmentId;
            bool isNew = !int.TryParse(hfAssignmentID.Value, out assignmentId) || assignmentId == 0;

            try
            {
                using (var conn = new SqlConnection(connString))
                using (var cmd = new SqlCommand(isNew
                    ? @"INSERT INTO Assignments (ModuleID, Title, Description, DueDate, MaxScore, IsActive)
                        VALUES (@ModuleID, @Title, @Description, @DueDate, @MaxScore, @IsActive)"
                    : @"UPDATE Assignments SET ModuleID = @ModuleID, Title = @Title, Description = @Description,
                               DueDate = @DueDate, MaxScore = @MaxScore, IsActive = @IsActive
                        WHERE AssignmentID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ModuleID", moduleId);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@DueDate", dueDate);
                    cmd.Parameters.AddWithValue("@MaxScore", maxScore);
                    cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);
                    if (!isNew)
                        cmd.Parameters.AddWithValue("@ID", assignmentId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Save assignment", ex.Message, ex.StackTrace);
                ShowFormError("Unable to save this assignment. Please try again.");
                return;
            }

            ShowSuccess(isNew ? "Assignment added." : "Assignment updated.");
            ClearForm();
            LoadAssignments();
            UiHelper.HideModal(this, "modalAssignment");
        }

        private void ShowFormError(string message)
        {
            pnlModalError.Visible = true;
            lblModalError.Text = Server.HtmlEncode(message);
            UiHelper.ShowModal(this, "modalAssignment");
        }

        private void ClearForm()
        {
            hfAssignmentID.Value = "";
            ddlModule.SelectedIndex = 0;
            txtTitle.Text = "";
            txtDescription.Text = "";
            txtDueDate.Text = "";
            txtMaxScore.Text = "";
            chkIsActive.Checked = true;
            lblModalTitle.Text = "Add Assignment";
            pnlModalError.Visible = false;
        }

        protected void btnAddAssignment_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalAssignment').modal('show');", true);
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
