using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class Lesson : Page
    {
        private static string connectionString => DatabaseHelper.ConnectionString;

        private sealed class LessonView
        {
            public LessonItem Lesson;
            public int? PreviousLessonId;
            public int? NextLessonId;
            public int ModuleLessonCount;
            public int ModuleCompletedCount;
        }

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
                    ShowNotFound();
                }
            }
        }

        private void ShowNotFound()
        {
            pnlLesson.Visible = false;
            pnlNotFound.Visible = true;
        }

        private void LoadLesson(int lessonId)
        {
            var view = GetLessonView(lessonId);
            if (view == null)
            {
                ShowNotFound();
                return;
            }

            var lesson = view.Lesson;
            Page.Title = lesson.Title;
            pnlLesson.Visible = true;
            lblLessonTitle.Text = Server.HtmlEncode(lesson.Title);
            lblLessonHeading.Text = Server.HtmlEncode(lesson.Title);
            hfModuleId.Value = lesson.ModuleId.ToString();
            lnkModule.Text = Server.HtmlEncode(lesson.ModuleTitle);
            lnkModule.NavigateUrl = "~/Learner/ModuleDetails.aspx?id=" + lesson.ModuleId;
            litContent.Text = FormatContent(lesson.Content);
            lblEstimatedTime.Text = EstimateMinutes(lesson.Content).ToString();

            BindVideo(lesson.VideoUrl);
            BindNavigation(view);
            BindCompletion(lesson.IsCompleted, view.ModuleCompletedCount, view.ModuleLessonCount);
            BindResources(lesson);
        }

        private LessonView GetLessonView(int lessonId)
        {
            var learnerId = SessionHelper.GetCurrentUserId();
            if (!learnerId.HasValue)
                return null;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"
                    SELECT l.LessonID, l.Title, l.ModuleID, l.Content, l.VideoUrl, l.ResourceUrl, m.Title AS ModuleTitle,
                           CAST(CASE WHEN EXISTS
                           (
                               SELECT 1 FROM LearnerProgress lp
                               WHERE lp.LearnerID = @LearnerID
                                 AND lp.LessonID = l.LessonID
                                 AND lp.Status IN (N'Completed', N'Certified')
                           ) THEN 1 ELSE 0 END AS BIT) AS IsCompleted
                    FROM Lessons l
                    INNER JOIN Modules m ON m.ModuleID = l.ModuleID
                    WHERE l.LessonID = @LessonID AND l.IsActive = 1 AND m.IsActive = 1;

                    -- Every active lesson in the same module, in learning order, for prev/next and progress.
                    SELECT l.LessonID,
                           CAST(CASE WHEN EXISTS
                           (
                               SELECT 1 FROM LearnerProgress lp
                               WHERE lp.LearnerID = @LearnerID
                                 AND lp.LessonID = l.LessonID
                                 AND lp.Status IN (N'Completed', N'Certified')
                           ) THEN 1 ELSE 0 END AS BIT) AS IsCompleted
                    FROM Lessons l
                    WHERE l.IsActive = 1
                      AND l.ModuleID = (SELECT ModuleID FROM Lessons WHERE LessonID = @LessonID)
                    ORDER BY l.LessonOrder, l.LessonID;", conn))
                {
                    cmd.Parameters.Add("@LessonID", SqlDbType.Int).Value = lessonId;
                    cmd.Parameters.Add("@LearnerID", SqlDbType.Int).Value = learnerId.Value;
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        var view = new LessonView
                        {
                            Lesson = new LessonItem
                            {
                                LessonID = Convert.ToInt32(reader["LessonID"]),
                                Title = Convert.ToString(reader["Title"]),
                                ModuleTitle = Convert.ToString(reader["ModuleTitle"]),
                                ModuleId = Convert.ToInt32(reader["ModuleID"]),
                                Content = Convert.ToString(reader["Content"]),
                                VideoUrl = Convert.ToString(reader["VideoUrl"]),
                                ResourceUrl = Convert.ToString(reader["ResourceUrl"]),
                                IsCompleted = Convert.ToBoolean(reader["IsCompleted"])
                            }
                        };

                        reader.NextResult();
                        var orderedIds = new List<int>();
                        while (reader.Read())
                        {
                            orderedIds.Add(Convert.ToInt32(reader["LessonID"]));
                            if (Convert.ToBoolean(reader["IsCompleted"]))
                                view.ModuleCompletedCount++;
                        }

                        view.ModuleLessonCount = orderedIds.Count;
                        int index = orderedIds.IndexOf(lessonId);
                        if (index > 0)
                            view.PreviousLessonId = orderedIds[index - 1];
                        if (index >= 0 && index < orderedIds.Count - 1)
                            view.NextLessonId = orderedIds[index + 1];

                        return view;
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Load lesson", ex.Message, ex.StackTrace);
                return null;
            }
        }

        // Lesson content is authored by admins. Plain text is converted to paragraphs;
        // content that already contains HTML markup is shown as-is.
        private static string FormatContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return "<p class=\"text-muted\">This lesson has no written content yet.</p>";

            if (Regex.IsMatch(content, @"<\s*(p|div|h[1-6]|ul|ol|br|img|table|strong|em|a)\b", RegexOptions.IgnoreCase))
                return content;

            var paragraphs = Regex.Split(content.Trim(), @"\r?\n\s*\r?\n");
            var html = new System.Text.StringBuilder();
            foreach (var paragraph in paragraphs)
            {
                html.Append("<p>")
                    .Append(HttpUtility.HtmlEncode(paragraph.Trim()).Replace("\r\n", "<br />").Replace("\n", "<br />"))
                    .Append("</p>");
            }
            return html.ToString();
        }

        private static int EstimateMinutes(string content)
        {
            string plain = Regex.Replace(content ?? string.Empty, "<.*?>", " ");
            int words = Regex.Matches(plain, @"\S+").Count;
            // About 200 words per minute, with a sensible minimum for short lessons.
            return Math.Max(5, (int)Math.Ceiling(words / 200.0));
        }

        private void BindVideo(string videoUrl)
        {
            string embedUrl = ModuleMediaHelper.GetYouTubeEmbedUrl(videoUrl);
            pnlLessonVideo.Visible = !string.IsNullOrEmpty(embedUrl);
            if (pnlLessonVideo.Visible)
                iframeLessonVideo.Attributes["src"] = embedUrl;
        }

        private void BindNavigation(LessonView view)
        {
            hlPrevLesson.Visible = view.PreviousLessonId.HasValue;
            if (view.PreviousLessonId.HasValue)
                hlPrevLesson.NavigateUrl = "~/Learner/Lesson.aspx?id=" + view.PreviousLessonId.Value;

            if (view.NextLessonId.HasValue)
            {
                hlNextLesson.NavigateUrl = "~/Learner/Lesson.aspx?id=" + view.NextLessonId.Value;
                litNextText.Text = "Next Lesson";
            }
            else
            {
                // Last lesson: send the learner back to the module, where the quiz unlocks.
                hlNextLesson.NavigateUrl = "~/Learner/ModuleDetails.aspx?id=" + view.Lesson.ModuleId;
                litNextText.Text = "Back to Module";
            }
        }

        private void BindCompletion(bool isCompleted, int completedCount, int totalCount)
        {
            lblStatus.Text = isCompleted ? "Completed" : "In Progress";
            lblStatus.CssClass = "badge " + (isCompleted ? "badge-success" : "badge-warning");
            lblLessonCompleteStatus.Text = isCompleted ? CompletedHtml() : string.Empty;
            btnMarkComplete.Visible = !isCompleted;

            int progress = totalCount == 0 ? 0 : (int)Math.Round(completedCount * 100.0 / totalCount);
            pnlLessonProgress.Style["width"] = progress + "%";
            lblProgressText.Text = string.Format("Module progress: {0} of {1} lessons completed ({2}%)", completedCount, totalCount, progress);
        }

        private string CompletedHtml()
        {
            return "<img src=\"" + ResolveUrl("~/Content/Images/icons/check.svg") + "\" alt=\"\" class=\"img-icon\" /> Completed";
        }

        private void BindResources(LessonItem lesson)
        {
            var resources = new List<ResourceItem>();
            if (IsWebUrl(lesson.ResourceUrl))
                resources.Add(new ResourceItem { Title = "Lesson resource", Url = lesson.ResourceUrl, Icon = "file-alt" });
            if (IsWebUrl(lesson.VideoUrl))
                resources.Add(new ResourceItem { Title = "Watch the lesson video", Url = lesson.VideoUrl, Icon = "video" });

            rptResources.DataSource = resources;
            rptResources.DataBind();
            lblNoResources.Visible = resources.Count == 0;
        }

        private static bool IsWebUrl(string value)
        {
            Uri uri;
            return !string.IsNullOrWhiteSpace(value) &&
                Uri.TryCreate(value, UriKind.Absolute, out uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        protected void btnMarkComplete_Click(object sender, EventArgs e)
        {
            int lessonId;
            var learnerId = SessionHelper.GetCurrentUserId();
            var view = int.TryParse(Request.QueryString["id"], out lessonId) && learnerId.HasValue
                ? GetLessonView(lessonId)
                : null;

            if (view == null)
            {
                UiHelper.Notify(this, "This lesson is no longer available.", "error");
                return;
            }

            if (!view.Lesson.IsCompleted)
            {
                try
                {
                    DatabaseHelper.EnsureLearnerProfile(learnerId.Value);

                    using (var conn = new SqlConnection(connectionString))
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
                        END;", conn))
                    {
                        cmd.Parameters.Add("@LearnerID", SqlDbType.Int).Value = learnerId.Value;
                        cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = view.Lesson.ModuleId;
                        cmd.Parameters.Add("@LessonID", SqlDbType.Int).Value = view.Lesson.LessonID;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    DatabaseHelper.LogError("Mark lesson complete", ex.Message, ex.StackTrace);
                    UiHelper.Notify(this, "Unable to save your progress. Please try again.", "error");
                    return;
                }

                view.ModuleCompletedCount++;
            }

            BindCompletion(true, view.ModuleCompletedCount, view.ModuleLessonCount);
            upLessonHeader.Update();
            UiHelper.Notify(this, view.NextLessonId.HasValue
                ? "Lesson completed. On to the next one!"
                : "Lesson completed. You've finished every lesson in this module.", "success");
        }
    }
}
