using System;
using System.Collections.Generic;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class Lesson : Page
    {
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
            var lessons = new Dictionary<int, LessonItem>
            {
                { 1, new LessonItem { LessonID = 1, Title = "What is Emergency Response?", ModuleTitle = "Introduction to Emergency Response", ModuleId = 1, Content = "<h2>Understanding Emergency Response</h2><p>Emergency response is the organized approach to addressing emergencies and disasters. It involves preparation, response, and recovery efforts to protect lives and property.</p><h3>Key Components</h3><ul><li>Assessment and evaluation</li><li>Resource mobilization</li><li>Communication and coordination</li><li>Action implementation</li></ul>", EstimatedTime = 15, IsCompleted = true } },
                { 2, new LessonItem { LessonID = 2, Title = "Assessment of Emergency Situations", ModuleTitle = "Introduction to Emergency Response", ModuleId = 1, Content = "<h2>Assessing Emergency Situations</h2><p>Proper assessment is crucial for effective emergency response. Learn how to evaluate the situation, identify risks, and prioritize actions.</p>", EstimatedTime = 20, IsCompleted = false } }
            };

            return lessons.ContainsKey(lessonId) ? lessons[lessonId] : null;
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
