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
        private static string connString => DatabaseHelper.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireAnyRole("Expert", "Admin"))
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
            int submissionId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out submissionId))
                return;

            if (e.CommandName == "Grade")
            {
                using (var conn = new SqlConnection(connString))
                {
                    var query = @"
                        SELECT s.Content, s.FileUrl, s.Score, s.Feedback, a.MaxScore
                        FROM AssignmentSubmissions s
                        INNER JOIN Assignments a ON a.AssignmentID = s.AssignmentID
                        WHERE s.SubmissionID = @ID";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", submissionId);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hfSubmissionID.Value = submissionId.ToString();
                                hfMaxScore.Value = reader["MaxScore"].ToString();
                                lblMaxScore.Text = "(out of " + reader["MaxScore"] + ")";
                                pnlModalError.Visible = false;
                                // Learner text is untrusted: encode it and keep line breaks.
                                litContent.Text = Server.HtmlEncode(reader["Content"].ToString()).Replace("\n", "<br />");

                                string fileUrl = reader["FileUrl"].ToString();
                                if (IsSafeLink(fileUrl))
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

        private static bool IsSafeLink(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;
            if (url.StartsWith("~/") || (url.StartsWith("/") && !url.StartsWith("//")))
                return true;

            Uri uri;
            return Uri.TryCreate(url, UriKind.Absolute, out uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        protected void btnSaveGrade_Click(object sender, EventArgs e)
        {
            int submissionId, score, maxScore;
            if (!int.TryParse(hfSubmissionID.Value, out submissionId))
            {
                ShowError("Select a submission to grade.");
                return;
            }
            if (!int.TryParse(hfMaxScore.Value, out maxScore))
                maxScore = int.MaxValue;
            if (!int.TryParse(txtScore.Text.Trim(), out score) || score < 0 || score > maxScore)
            {
                ShowGradeError(maxScore == int.MaxValue
                    ? "Enter a score of 0 or higher."
                    : "Enter a score between 0 and " + maxScore + ".");
                return;
            }

            try
            {
                using (var conn = new SqlConnection(connString))
                using (var cmd = new SqlCommand(@"
                    UPDATE AssignmentSubmissions
                    SET Score = @Score, Feedback = @Feedback, Status = N'Graded'
                    WHERE SubmissionID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@Feedback", txtFeedback.Text.Trim());
                    cmd.Parameters.AddWithValue("@ID", submissionId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Save grade", ex.Message, ex.StackTrace);
                ShowGradeError("Unable to save this grade. Please try again.");
                return;
            }

            ShowSuccess("Grade saved.");
            LoadSubmissions();
            UiHelper.HideModal(this, "modalGrade");
        }

        private void ShowGradeError(string message)
        {
            pnlModalError.Visible = true;
            lblModalError.Text = Server.HtmlEncode(message);
            UiHelper.ShowModal(this, "modalGrade");
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
