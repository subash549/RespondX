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
    public partial class GradeAssignments : Page
    {
        private string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString ?? "Data Source=DESKTOP-5UH7Q5H\\SQLEXPRESS01;Initial Catalog=RespondX;Integrated Security=True;TrustServerCertificate=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Expert") && !AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                LoadSubmissions();
            }
        }

        private void LoadSubmissions()
        {
            var submissions = new List<AssignmentSubmissionItem>();
            var user = SessionHelper.GetCurrentUser();

            using (var conn = new SqlConnection(connString))
            {
                var query = @"
                    SELECT s.SubmissionID, s.AssignmentID, a.Title AS AssignmentTitle, 
                           s.LearnerID, u.FirstName + ' ' + u.LastName AS LearnerName,
                           s.Status, s.SubmittedAt
                    FROM AssignmentSubmissions s
                    INNER JOIN Assignments a ON s.AssignmentID = a.AssignmentID
                    INNER JOIN Modules m ON a.ModuleID = m.ModuleID
                    INNER JOIN Users u ON s.LearnerID = u.UserID";
                
                if (user.Role != "Admin")
                {
                    query += " WHERE m.InstructorID = @ExpertID";
                }
                query += " ORDER BY s.SubmittedAt DESC";
                
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
                            submissions.Add(new AssignmentSubmissionItem
                            {
                                SubmissionID = Convert.ToInt32(reader["SubmissionID"]),
                                AssignmentTitle = reader["AssignmentTitle"].ToString(),
                                LearnerName = reader["LearnerName"].ToString(),
                                Status = reader["Status"].ToString(),
                                SubmittedAt = Convert.ToDateTime(reader["SubmittedAt"])
                            });
                        }
                    }
                }
            }

            rptSubmissions.DataSource = submissions;
            rptSubmissions.DataBind();
        }

        protected void rptSubmissions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int submissionId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Grade")
            {
                using (var conn = new SqlConnection(connString))
                {
                    var query = "SELECT Content, FileUrl, Score, Feedback FROM AssignmentSubmissions WHERE SubmissionID = @ID";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", submissionId);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hfSubmissionID.Value = submissionId.ToString();
                                litContent.Text = reader["Content"].ToString();
                                
                                string fileUrl = reader["FileUrl"].ToString();
                                if (!string.IsNullOrEmpty(fileUrl))
                                {
                                    divFile.Visible = true;
                                    hlFile.NavigateUrl = fileUrl;
                                }
                                else
                                {
                                    divFile.Visible = false;
                                }
                                
                                txtScore.Text = reader["Score"] != DBNull.Value ? reader["Score"].ToString() : "";
                                txtFeedback.Text = reader["Feedback"].ToString();

                                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalGrade').modal('show');", true);
                            }
                        }
                    }
                }
            }
        }

        protected void btnSaveGrade_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtScore.Text))
            {
                ShowError("Score is required.");
                return;
            }

            int submissionId = int.Parse(hfSubmissionID.Value);

            using (var conn = new SqlConnection(connString))
            {
                var query = @"
                    UPDATE AssignmentSubmissions 
                    SET Score = @Score, Feedback = @Feedback, Status = 'Graded'
                    WHERE SubmissionID = @ID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Score", int.Parse(txtScore.Text));
                    cmd.Parameters.AddWithValue("@Feedback", txtFeedback.Text.Trim());
                    cmd.Parameters.AddWithValue("@ID", submissionId);
                    
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ShowSuccess("Grade saved successfully");
            LoadSubmissions();

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalGrade').modal('hide');", true);
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
