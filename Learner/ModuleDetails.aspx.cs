using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class ModuleDetails : Page
    {
        private static string connString => DatabaseHelper.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                int moduleId;
                if (int.TryParse(Request.QueryString["id"], out moduleId))
                {
                    LoadModuleDetails(moduleId);
                }
                else
                {
                    pnlModule.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadModuleDetails(int moduleId)
        {
            var user = SessionHelper.GetCurrentUser();
            ModuleItem module = null;

            using (var conn = new SqlConnection(connString))
            {
                var query = @"
                    SELECT m.ModuleID, m.Title, m.Description, m.EstimatedHours, m.ThumbnailUrl,
                           m.YouTubeVideoUrl, m.VideoFileUrl,
                           c.Name AS CategoryName, u.FirstName + ' ' + u.LastName AS InstructorName,
                           (SELECT COUNT(*) FROM Lessons l WHERE l.ModuleID = m.ModuleID AND l.IsActive = 1) AS TotalLessons,
                           (SELECT COUNT(*) FROM LearnerProgress lp 
                            INNER JOIN Lessons l2 ON lp.LessonID = l2.LessonID 
                            WHERE l2.ModuleID = m.ModuleID AND lp.LearnerID = @LearnerID
                              AND lp.Status IN (N'Completed', N'Certified')) AS CompletedLessons
                    FROM Modules m
                    LEFT JOIN Categories c ON m.CategoryID = c.CategoryID
                    LEFT JOIN Users u ON m.InstructorID = u.UserID
                    WHERE m.ModuleID = @ModuleID AND m.IsActive = 1";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ModuleID", moduleId);
                    cmd.Parameters.AddWithValue("@LearnerID", user.UserID);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int totalLessons = Convert.ToInt32(reader["TotalLessons"]);
                            int completedLessons = Convert.ToInt32(reader["CompletedLessons"]);
                            int progress = totalLessons > 0 ? (int)Math.Round((double)completedLessons / totalLessons * 100) : 0;
                            bool isCompleted = totalLessons > 0 && completedLessons == totalLessons;

                            module = new ModuleItem
                            {
                                ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                EstimatedHours = reader["EstimatedHours"] != DBNull.Value ? Convert.ToInt32(reader["EstimatedHours"]) : 0,
                                ThumbnailUrl = reader["ThumbnailUrl"] != DBNull.Value ? reader["ThumbnailUrl"].ToString() : "",
                                YouTubeVideoUrl = reader["YouTubeVideoUrl"] != DBNull.Value ? reader["YouTubeVideoUrl"].ToString() : "",
                                VideoFileUrl = reader["VideoFileUrl"] != DBNull.Value ? reader["VideoFileUrl"].ToString() : "",
                                CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : "Uncategorized",
                                InstructorName = reader["InstructorName"] != DBNull.Value ? reader["InstructorName"].ToString() : "Not Assigned",
                                LessonCount = totalLessons,
                                ProgressPercentage = progress,
                                IsCompleted = isCompleted,
                                Status = isCompleted ? "Completed" : (progress > 0 ? "In Progress" : "Not Started"),
                                StatusBadge = isCompleted ? "badge-success" : (progress > 0 ? "badge-warning" : "badge-secondary"),
                                CompletedLessons = completedLessons,
                                TotalLessons = totalLessons
                            };
                        }
                    }
                }
            }

            if (module == null)
            {
                pnlModule.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            pnlModule.Visible = true;
            lblModuleTitle.Text = module.Title;
            lblModuleDescription.Text = module.Description;
            lblEstimatedHours.Text = module.EstimatedHours.ToString();
            lblLessonCount.Text = module.LessonCount.ToString();
            lblStatusBadge.Text = module.Status;
            lblStatusBadge.CssClass = "badge " + module.StatusBadge;
            lblCategory.Text = module.CategoryName;
            lblInstructor.Text = module.InstructorName;
            lblProgressDisplay.Text = module.ProgressPercentage + "%";
            pnlProgressBar.Style["width"] = module.ProgressPercentage + "%";
            lblCompletedLessons.Text = module.CompletedLessons.ToString();
            lblTotalLessons.Text = module.TotalLessons.ToString();
            
            imgModuleCover.ImageUrl = string.IsNullOrEmpty(module.ThumbnailUrl) ? "~/Content/Images/default-module.svg" : module.ThumbnailUrl;

            BindModuleMedia(module);

            LoadLessons(moduleId, user.UserID);
            LoadScenarios(moduleId);
            LoadQuiz(moduleId, module.Title);
        }

        private void BindModuleMedia(ModuleItem module)
        {
            var youtubeEmbed = ModuleMediaHelper.GetYouTubeEmbedUrl(module.YouTubeVideoUrl);
            if (!string.IsNullOrEmpty(youtubeEmbed))
            {
                pnlYouTubeVideo.Visible = true;
                iframeYouTube.Attributes["src"] = youtubeEmbed;
            }
            else
            {
                pnlYouTubeVideo.Visible = false;
                iframeYouTube.Attributes.Remove("src");
            }

            if (!string.IsNullOrEmpty(module.VideoFileUrl))
            {
                pnlUploadedVideo.Visible = true;
                videoUploaded.Attributes["src"] = ResolveUrl(module.VideoFileUrl);
            }
            else
            {
                pnlUploadedVideo.Visible = false;
                videoUploaded.Attributes.Remove("src");
            }

            pnlNoModuleVideo.Visible = string.IsNullOrEmpty(youtubeEmbed) && string.IsNullOrEmpty(module.VideoFileUrl);
        }

        private void LoadLessons(int moduleId, int learnerId)
        {
            var lessons = new List<LessonItem>();
            
            using (var conn = new SqlConnection(connString))
            {
                var query = @"
                    SELECT l.LessonID, l.Title, l.Content, l.LessonOrder,
                           CAST(CASE WHEN EXISTS
                           (
                               SELECT 1 FROM LearnerProgress lp
                               WHERE lp.LessonID = l.LessonID
                                 AND lp.LearnerID = @LearnerID
                                 AND lp.Status IN (N'Completed', N'Certified')
                           ) THEN 1 ELSE 0 END AS BIT) AS IsCompleted
                    FROM Lessons l
                    WHERE l.ModuleID = @ModuleID AND l.IsActive = 1
                    ORDER BY l.LessonOrder";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ModuleID", moduleId);
                    cmd.Parameters.AddWithValue("@LearnerID", learnerId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var content = reader["Content"] != DBNull.Value ? reader["Content"].ToString() : "";
                            lessons.Add(new LessonItem
                            {
                                LessonID = Convert.ToInt32(reader["LessonID"]),
                                Title = reader["Title"].ToString(),
                                Description = ModuleMediaHelper.GetLessonSummary(content),
                                IsCompleted = Convert.ToBoolean(reader["IsCompleted"])
                            });
                        }
                    }
                }
            }

            rptLessons.DataSource = lessons;
            rptLessons.DataBind();
        }

        private void LoadScenarios(int moduleId)
        {
            var scenarios = new List<ScenarioItem>();
            
            using (var conn = new SqlConnection(connString))
            {
                var query = @"
                    SELECT ScenarioID, Title, Description, DifficultyLevel 
                    FROM Scenarios 
                    WHERE ModuleID = @ModuleID AND IsActive = 1";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ModuleID", moduleId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int difficultyLevel = reader["DifficultyLevel"] != DBNull.Value
                                ? Convert.ToInt32(reader["DifficultyLevel"])
                                : 1;
                            scenarios.Add(new ScenarioItem
                            {
                                ScenarioID = Convert.ToInt32(reader["ScenarioID"]),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                Difficulty = GetDifficultyLabel(difficultyLevel)
                            });
                        }
                    }
                }
            }

            rptScenarios.DataSource = scenarios;
            rptScenarios.DataBind();
        }

        private void LoadQuiz(int moduleId, string moduleTitle)
        {
            bool hasQuiz = false;
            using (var conn = new SqlConnection(connString))
            {
                var query = "SELECT COUNT(1) FROM Quizzes WHERE ModuleID = @ModuleID AND IsActive = 1";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ModuleID", moduleId);
                    conn.Open();
                    hasQuiz = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }

            if (!hasQuiz)
            {
                hasQuiz = QuizCatalog.GetAll().Exists(definition =>
                    string.Equals(definition.ModuleTitle.Trim(), moduleTitle.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (hasQuiz)
            {
                pnlQuiz.Visible = true;
                pnlNoQuiz.Visible = false;
                bool moduleCompleted = QuizAccessHelper.IsModuleCompleted(moduleId, SessionHelper.GetCurrentUserId().GetValueOrDefault());
                lblQuizInfo.Text = moduleCompleted
                    ? "Test your knowledge with this module quiz."
                    : "Complete every active lesson in this module to unlock its quiz.";
                btnTakeQuiz.Enabled = moduleCompleted;
                btnTakeQuiz.CommandArgument = moduleId.ToString();
            }
            else
            {
                pnlQuiz.Visible = false;
                pnlNoQuiz.Visible = true;
            }
        }

        protected void btnTakeQuiz_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            int moduleId;
            var learnerId = SessionHelper.GetCurrentUserId();
            if (btn != null && int.TryParse(btn.CommandArgument, out moduleId) && learnerId.HasValue &&
                QuizAccessHelper.IsModuleCompleted(moduleId, learnerId.Value))
            {
                Response.Redirect($"~/Learner/Quizzes.aspx?moduleId={moduleId}", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private static string GetDifficultyLabel(int level)
        {
            switch (level)
            {
                case 1: return "Beginner";
                case 2: return "Easy";
                case 3: return "Intermediate";
                case 4: return "Advanced";
                case 5: return "Expert";
                default: return "Intermediate";
            }
        }
    }
}
