using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class Modules : Page
    {
        private string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString ?? "Data Source=DESKTOP-5UH7Q5H\\SQLEXPRESS01;Initial Catalog=RespondX;Integrated Security=True;TrustServerCertificate=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                LoadModules();
            }
        }

        private void LoadModules()
        {
            var modules = GetModulesFromDB();

            var filter = ddlFilter.SelectedValue;
            if (filter != "All")
            {
                if (filter == "Completed")
                {
                    modules = modules.FindAll(m => m.IsCompleted);
                }
                else if (filter == "InProgress")
                {
                    modules = modules.FindAll(m => m.ProgressPercentage > 0 && !m.IsCompleted);
                }
                else if (filter == "NotStarted")
                {
                    modules = modules.FindAll(m => m.ProgressPercentage == 0);
                }
            }

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                modules = modules.FindAll(m =>
                    m.Title.ToLower().Contains(search.ToLower()) ||
                    m.Description.ToLower().Contains(search.ToLower()) ||
                    m.CategoryName.ToLower().Contains(search.ToLower())
                );
            }

            rptModules.DataSource = modules;
            rptModules.DataBind();
        }

        private List<ModuleItem> GetModulesFromDB()
        {
            var modules = new List<ModuleItem>();
            var user = SessionHelper.GetCurrentUser();

            using (var conn = new SqlConnection(connString))
            {
                // This query fetches modules and calculates progress based on LearnerProgress table
                var query = @"
                    SELECT m.ModuleID, m.Title, m.Description, m.EstimatedHours, m.ThumbnailUrl, 
                           c.Name AS CategoryName,
                           (SELECT COUNT(*) FROM Lessons l WHERE l.ModuleID = m.ModuleID AND l.IsActive = 1) AS TotalLessons,
                           (SELECT COUNT(*) FROM LearnerProgress lp 
                            INNER JOIN Lessons l2 ON lp.LessonID = l2.LessonID 
                            WHERE l2.ModuleID = m.ModuleID AND lp.LearnerID = @LearnerID
                              AND lp.Status IN (N'Completed', N'Certified')) AS CompletedLessons
                    FROM Modules m
                    LEFT JOIN Categories c ON m.CategoryID = c.CategoryID
                    WHERE m.IsActive = 1
                    ORDER BY m.ModuleOrder";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LearnerID", user.UserID);
                    conn.Open();
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int totalLessons = Convert.ToInt32(reader["TotalLessons"]);
                            int completedLessons = Convert.ToInt32(reader["CompletedLessons"]);
                            int progress = totalLessons > 0 ? (int)Math.Round((double)completedLessons / totalLessons * 100) : 0;
                            bool isCompleted = totalLessons > 0 && completedLessons == totalLessons;
                            
                            string status = isCompleted ? "Completed" : (progress > 0 ? "In Progress" : "Not Started");
                            string statusBadge = isCompleted ? "badge-success" : (progress > 0 ? "badge-warning" : "badge-secondary");

                            modules.Add(new ModuleItem
                            {
                                ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                EstimatedHours = reader["EstimatedHours"] != DBNull.Value ? Convert.ToInt32(reader["EstimatedHours"]) : 0,
                                ThumbnailUrl = reader["ThumbnailUrl"].ToString(),
                                CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : "Uncategorized",
                                LessonCount = totalLessons,
                                ProgressPercentage = progress,
                                IsCompleted = isCompleted,
                                Status = status,
                                StatusBadge = statusBadge
                            });
                        }
                    }
                }
            }
            return modules;
        }

        protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadModules();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadModules();
        }
    }
}
