using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class LearnerDashboard : Page
    {
        private string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString ?? "Data Source=DESKTOP-5UH7Q5H\\SQLEXPRESS01;Initial Catalog=RespondX;Integrated Security=True;TrustServerCertificate=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            var userId = SessionHelper.GetCurrentUserId();
            if (!userId.HasValue) return;

            var learner = DatabaseHelper.GetLearnerById(userId.Value);
            if (learner != null)
            {
                lblUserName.Text = learner.FullName;
            }

            LoadStats(userId.Value);
            LoadRecentModules(userId.Value);
            LoadRecentActivity(userId.Value);
            LoadAlerts(userId.Value);
        }

        private void LoadStats(int learnerId)
        {
            lblTotalModules.Text = "8";
            lblCompletedModules.Text = "3";
            lblInProgressModules.Text = "2";
            lblCertificates.Text = "2";
        }

        private void LoadRecentModules(int learnerId)
        {
            var modules = new List<ModuleItem>();

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    var query = @"
                        SELECT TOP 5 m.ModuleID, m.Title,
                               (SELECT COUNT(*) FROM Lessons l WHERE l.ModuleID = m.ModuleID AND l.IsActive = 1) AS TotalLessons,
                               (SELECT COUNT(*) FROM LearnerProgress lp
                                INNER JOIN Lessons l2 ON lp.LessonID = l2.LessonID
                                WHERE l2.ModuleID = m.ModuleID AND lp.LearnerID = @LearnerID
                                  AND lp.Status IN (N'Completed', N'Certified')) AS CompletedLessons
                        FROM Modules m
                        WHERE m.IsActive = 1
                        ORDER BY m.ModuleOrder, m.ModuleID";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LearnerID", learnerId);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int totalLessons = Convert.ToInt32(reader["TotalLessons"]);
                                int completedLessons = Convert.ToInt32(reader["CompletedLessons"]);
                                int progress = totalLessons > 0 ? (int)Math.Round((double)completedLessons / totalLessons * 100) : 0;
                                bool isCompleted = totalLessons > 0 && completedLessons == totalLessons;

                                modules.Add(new ModuleItem
                                {
                                    ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                    Title = reader["Title"].ToString(),
                                    Status = isCompleted ? "Completed" : (progress > 0 ? "In Progress" : "Not Started"),
                                    ProgressPercentage = progress
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Keep sign-in usable even if module/progress tables are unavailable.
                DatabaseHelper.LogError("Load learner dashboard modules", ex.Message, ex.StackTrace);
            }

            rptRecentModules.DataSource = modules;
            rptRecentModules.DataBind();
        }

        private void LoadRecentActivity(int learnerId)
        {
            var activities = new List<ActivityItem>
            {
                new ActivityItem { Title = "Completed Module 2", Description = "Finished CPR and First Aid module", ActivityType = "completed", Icon = "check-circle", TimeAgo = "2 hours ago" },
                new ActivityItem { Title = "Started Quiz", Description = "Took Emergency Response quiz", ActivityType = "quiz", Icon = "question-circle", TimeAgo = "5 hours ago" },
                new ActivityItem { Title = "Practiced Scenario", Description = "Completed emergency communication scenario", ActivityType = "scenario", Icon = "users", TimeAgo = "1 day ago" }
            };

            rptRecentActivity.DataSource = activities;
            rptRecentActivity.DataBind();
        }

        private void LoadAlerts(int learnerId)
        {
            var alerts = new List<AlertItem>
            {
                new AlertItem { Title = "New Module Available", Message = "Advanced Emergency Response has been added", AlertType = "notification", CreatedAt = DateTime.Now.AddHours(-3), TimeAgo = "3 hours ago" },
                new AlertItem { Title = "Reminder", Message = "Complete your CPR certification quiz", AlertType = "reminder", CreatedAt = DateTime.Now.AddDays(-1), TimeAgo = "1 day ago" }
            };

            rptAlerts.DataSource = alerts;
            rptAlerts.DataBind();
        }
    }
}
