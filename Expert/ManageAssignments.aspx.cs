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
        private string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString ?? "Data Source=DESKTOP-5UH7Q5H\\SQLEXPRESS01;Initial Catalog=RespondX;Integrated Security=True;TrustServerCertificate=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Expert") && !AuthorizationHelper.RequireRole("Admin"))
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
            int assignmentId = int.Parse(e.CommandArgument.ToString());

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
                            lblModalTitle.Text = "Edit Assignment";
                            hfAssignmentID.Value = assignmentId.ToString();
                            ddlModule.SelectedValue = reader["ModuleID"].ToString();
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
            using (var conn = new SqlConnection(connString))
            {
                using (var cmd = new SqlCommand("UPDATE Assignments SET IsActive = ~IsActive WHERE AssignmentID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", assignmentId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            ShowSuccess("Assignment status updated successfully");
            LoadAssignments();
        }

        protected void btnSaveAssignment_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()) || string.IsNullOrEmpty(ddlModule.SelectedValue))
            {
                ShowError("Title and Module are required.");
                return;
            }

            int assignmentId;
            bool isNew = !int.TryParse(hfAssignmentID.Value, out assignmentId) || assignmentId == 0;

            using (var conn = new SqlConnection(connString))
            {
                string query = isNew ? 
                    @"INSERT INTO Assignments (ModuleID, Title, Description, DueDate, MaxScore, IsActive) 
                      VALUES (@ModuleID, @Title, @Description, @DueDate, @MaxScore, @IsActive)" : 
                    @"UPDATE Assignments SET ModuleID = @ModuleID, Title = @Title, Description = @Description, 
                             DueDate = @DueDate, MaxScore = @MaxScore, IsActive = @IsActive 
                      WHERE AssignmentID = @ID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ModuleID", int.Parse(ddlModule.SelectedValue));
                    cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@DueDate", string.IsNullOrEmpty(txtDueDate.Text) ? DateTime.Now.AddDays(7) : DateTime.Parse(txtDueDate.Text));
                    cmd.Parameters.AddWithValue("@MaxScore", string.IsNullOrEmpty(txtMaxScore.Text) ? 100 : int.Parse(txtMaxScore.Text));
                    cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);
                    
                    if (!isNew)
                    {
                        cmd.Parameters.AddWithValue("@ID", assignmentId);
                    }
                    
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ShowSuccess(isNew ? "Assignment added successfully" : "Assignment updated successfully");
            ClearForm();
            LoadAssignments();

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalAssignment').modal('hide');", true);
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
