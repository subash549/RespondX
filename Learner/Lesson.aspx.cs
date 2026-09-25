using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class Lesson : Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
            ?? "Data Source=DESKTOP-5UH7Q5H\\SQLEXPRESS01;Initial Catalog=RespondX;Integrated Security=True;TrustServerCertificate=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                int lessonId;
                if (int.TryParse(Request.QueryString["id"], out lessonId))
                {
                    LoadLesson(lessonId);
                }
                else
                {
                    pnlLesson.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadLesson(int lessonId)
        {
            var lesson = GetLesson(lessonId);
            if (lesson == null)
            {
                pnlLesson.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            pnlLesson.Visible = true;
            lblLessonTitle.Text = lesson.Title;
            lblLessonHeading.Text = lesson.Title;
            hfModuleId.Value = lesson.ModuleId.ToString();
            lnkModule.Text = Server.HtmlEncode(lesson.ModuleTitle);
            lnkModule.NavigateUrl = ResolveUrl("~/Learner/ModuleDetails.aspx?id=" + lesson.ModuleId);
            litContent.Text = lesson.Content;
            lblEstimatedTime.Text = lesson.EstimatedTime.ToString();
            lblStatus.Text = lesson.IsCompleted ? "Completed" : "In Progress";
            lblStatus.CssClass = "badge " + (lesson.IsCompleted ? "badge-success" : "badge-warning");
            lblLessonCompleteStatus.Text = lesson.IsCompleted ? "✓ Completed" : "";
            btnMarkComplete.Enabled = !lesson.IsCompleted;
            btnMarkComplete.Text = lesson.IsCompleted ? "Completed" : "Mark as Complete";

            int progress = lesson.IsCompleted ? 100 : 50;
            pnlLessonProgress.Style["width"] = progress + "%";
            lblProgressText.Text = $"Lesson Progress: {progress}%";

            LoadResources(lessonId);
        }

        private LessonItem GetLesson(int lessonId)
        {
            var learnerId = SessionHelper.GetCurrentUserId();
            if (!learnerId.HasValue)
                return null;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"
                    SELECT l.LessonID, l.Title, l.ModuleID, l.Content, m.Title AS ModuleTitle,
                           CAST(CASE WHEN EXISTS
                           (
                               SELECT 1 FROM LearnerProgress lp
                               WHERE lp.LearnerID = @LearnerID
                                 AND lp.LessonID = l.LessonID
                                 AND lp.Status IN (N'Completed', N'Certified')
                           ) THEN 1 ELSE 0 END AS BIT) AS IsCompleted
                    FROM Lessons l
                    INNER JOIN Modules m ON m.ModuleID = l.ModuleID
                    WHERE l.LessonID = @LessonID AND l.IsActive = 1 AND m.IsActive = 1;", conn))
                {
                    cmd.Parameters.Add("@LessonID", SqlDbType.Int).Value = lessonId;
                    cmd.Parameters.Add("@LearnerID", SqlDbType.Int).Value = learnerId.Value;
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        return new LessonItem
                        {
                            LessonID = Convert.ToInt32(reader["LessonID"]),
                            Title = reader["Title"].ToString(),
                            ModuleTitle = reader["ModuleTitle"].ToString(),
                            ModuleId = Convert.ToInt32(reader["ModuleID"]),
                            Content = reader["Content"] == DBNull.Value ? string.Empty : reader["Content"].ToString(),
                            EstimatedTime = 15,
                            IsCompleted = Convert.ToBoolean(reader["IsCompleted"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Load lesson", ex.Message, ex.StackTrace);
                return null;
            }

        }

        private void LoadResources(int lessonId)
        {
            var resources = new List<ResourceItem>
            {
                new ResourceItem { Title = "Emergency Response Guide", Url = "#", Icon = "file-pdf" },
                new ResourceItem { Title = "Video Tutorial", Url = "#", Icon = "video" }
            };

            rptResources.DataSource = resources;
            rptResources.DataBind();
        }

        protected void btnPrevLesson_Click(object sender, EventArgs e)
        {
            Response.Redirect("Lesson.aspx?id=1");
        }

        protected void btnNextLesson_Click(object sender, EventArgs e)
        {
            Response.Redirect("Lesson.aspx?id=2");
        }

        protected void btnMarkComplete_Click(object sender, EventArgs e)
        {
            int lessonId;
            var learnerId = SessionHelper.GetCurrentUserId();
            var lesson = int.TryParse(Request.QueryString["id"], out lessonId) && learnerId.HasValue
                ? GetLesson(lessonId)
                : null;

            if (lesson == null || lesson.IsCompleted)
                return;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    using (var cmd = new SqlCommand(@"
                        UPDATE LearnerProgress
                        SET Status = N'Completed',
                            ProgressPercentage = 100,
                            CompletedAt = ISNULL(CompletedAt, GETDATE()),
                            LastAccessedAt = GETDATE()
                        WHERE LearnerID = @LearnerID AND ModuleID = @ModuleID AND LessonID = @LessonID;

                        IF @@ROWCOUNT = 0
                        BEGIN
                            INSERT INTO LearnerProgress
                                (LearnerID, ModuleID, LessonID, Status, StartedAt, CompletedAt, LastAccessedAt, ProgressPercentage, TimeSpentMinutes)
                            VALUES
                                (@LearnerID, @ModuleID, @LessonID, N'Completed', GETDATE(), GETDATE(), GETDATE(), 100, 0);
                        END;", conn, transaction))
                    {
                        cmd.Parameters.Add("@LearnerID", SqlDbType.Int).Value = learnerId.Value;
                        cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = lesson.ModuleId;
                        cmd.Parameters.Add("@LessonID", SqlDbType.Int).Value = lesson.LessonID;
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Mark lesson complete", ex.Message, ex.StackTrace);
                lblLessonCompleteStatus.Text = "Unable to save completion. Please try again.";
                lblLessonCompleteStatus.CssClass = "text-danger ml-1";
                return;
            }

            ShowNotification("Lesson marked as complete!", "success");
            btnMarkComplete.Enabled = false;
            btnMarkComplete.Text = "Completed";
            lblStatus.Text = "Completed";
            lblStatus.CssClass = "badge badge-success";
            lblLessonCompleteStatus.Text = "✓ Completed";
            pnlLessonProgress.Style["width"] = "100%";
            lblProgressText.Text = "Lesson Progress: 100%";
        }

        private void ShowNotification(string message, string type)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "notify",
                $"showNotification('{message}', '{type}');", true);
        }
    }
}
