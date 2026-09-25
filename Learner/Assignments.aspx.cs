using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;

namespace RespondX.Learner
{
    public partial class Assignments : Page
    {
        private static string connString => DatabaseHelper.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                LoadAssignments();
            }
        }

        private void LoadAssignments()
        {
            var user = SessionHelper.GetCurrentUser();
            var assignments = new List<LearnerAssignmentItem>();
            string filter = ddlFilter.SelectedValue;

            using (var conn = new SqlConnection(connString))
            {
                var query = @"
                    SELECT a.AssignmentID, a.Title, a.Description, a.DueDate, a.MaxScore, m.Title AS ModuleTitle,
                           s.SubmissionID, s.Status, s.Score, s.Feedback
                    FROM Assignments a
                    INNER JOIN Modules m ON a.ModuleID = m.ModuleID
                    LEFT JOIN AssignmentSubmissions s ON a.AssignmentID = s.AssignmentID AND s.LearnerID = @LearnerID
                    WHERE a.IsActive = 1 AND m.IsActive = 1";
                
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LearnerID", user.UserID);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : "Pending";
                            string badge = status == "Pending" ? "badge-warning" : (status == "Graded" ? "badge-success" : "badge-info");
                            
                            // Apply filter
                            if (filter != "All" && status != filter)
                                continue;

                            assignments.Add(new LearnerAssignmentItem
                            {
                                AssignmentID = Convert.ToInt32(reader["AssignmentID"]),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                ModuleTitle = reader["ModuleTitle"].ToString(),
                                DueDate = Convert.ToDateTime(reader["DueDate"]),
                                MaxScore = Convert.ToInt32(reader["MaxScore"]),
                                Status = status,
                                StatusBadge = badge,
                                Score = reader["Score"] != DBNull.Value ? Convert.ToInt32(reader["Score"]) : 0,
                                Feedback = reader["Feedback"] != DBNull.Value ? reader["Feedback"].ToString() : "",
                                HasSubmission = reader["SubmissionID"] != DBNull.Value
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
            if (e.CommandName == "Submit")
            {
                int assignmentId = int.Parse(e.CommandArgument.ToString());
                var user = SessionHelper.GetCurrentUser();

                using (var conn = new SqlConnection(connString))
                {
                    var query = @"
                        SELECT a.Title, a.Description, s.Content, s.FileUrl
                        FROM Assignments a
                        LEFT JOIN AssignmentSubmissions s ON a.AssignmentID = s.AssignmentID AND s.LearnerID = @LearnerID
                        WHERE a.AssignmentID = @AssignmentID";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LearnerID", user.UserID);
                        cmd.Parameters.AddWithValue("@AssignmentID", assignmentId);
                        
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hfAssignmentID.Value = assignmentId.ToString();
                                lblModalAssignmentTitle.InnerText = reader["Title"].ToString();
                                lblModalAssignmentDesc.InnerText = reader["Description"].ToString();
                                txtContent.Text = reader["Content"] != DBNull.Value ? reader["Content"].ToString() : "";
                                txtFileUrl.Text = reader["FileUrl"] != DBNull.Value ? reader["FileUrl"].ToString() : "";
                                
                                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalSubmit').modal('show');", true);
                            }
                        }
                    }
                }
            }
        }

        protected void btnSaveSubmission_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtContent.Text.Trim()))
            {
                ShowError("Submission content is required.");
                return;
            }

            int assignmentId = int.Parse(hfAssignmentID.Value);
            var user = SessionHelper.GetCurrentUser();

            using (var conn = new SqlConnection(connString))
            {
                // Check if submission exists
                var checkCmd = new SqlCommand("SELECT COUNT(1) FROM AssignmentSubmissions WHERE AssignmentID = @AssignmentID AND LearnerID = @LearnerID", conn);
                checkCmd.Parameters.AddWithValue("@AssignmentID", assignmentId);
                checkCmd.Parameters.AddWithValue("@LearnerID", user.UserID);
                
                conn.Open();
                bool exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                string query = exists ? 
                    @"UPDATE AssignmentSubmissions SET Content = @Content, FileUrl = @FileUrl, SubmittedAt = GETDATE(), Status = 'Submitted' 
                      WHERE AssignmentID = @AssignmentID AND LearnerID = @LearnerID" :
                    @"INSERT INTO AssignmentSubmissions (AssignmentID, LearnerID, Content, FileUrl, SubmittedAt, Status) 
                      VALUES (@AssignmentID, @LearnerID, @Content, @FileUrl, GETDATE(), 'Submitted')";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AssignmentID", assignmentId);
                    cmd.Parameters.AddWithValue("@LearnerID", user.UserID);
                    cmd.Parameters.AddWithValue("@Content", txtContent.Text.Trim());
                    cmd.Parameters.AddWithValue("@FileUrl", string.IsNullOrEmpty(txtFileUrl.Text) ? (object)DBNull.Value : txtFileUrl.Text.Trim());
                    
                    cmd.ExecuteNonQuery();
                }
            }

            ShowSuccess("Assignment submitted successfully");
            LoadAssignments();
            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalSubmit').modal('hide');", true);
        }

        protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAssignments();
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

    public class LearnerAssignmentItem
    {
        public int AssignmentID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ModuleTitle { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxScore { get; set; }
        public string Status { get; set; }
        public string StatusBadge { get; set; }
        public int Score { get; set; }
        public string Feedback { get; set; }
        public bool HasSubmission { get; set; }
    }
}
