using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageLessons : Page
    {
        private int moduleId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                if (int.TryParse(Request.QueryString["moduleId"], out moduleId))
                {
                    hfModuleID.Value = moduleId.ToString();
                    LoadModuleInfo();
                    LoadLessons();
                }
                else
                {
                    Response.Redirect("ManageModules.aspx");
                }
            }
            else
            {
                moduleId = int.Parse(hfModuleID.Value);
            }
        }

        private void LoadModuleInfo()
        {
            var module = GetModuleById(moduleId);
            if (module != null)
            {
                lblModuleInfo.Text = $"Module: {module.Title}";
            }
        }

        private void LoadLessons()
        {
            var lessons = GetLessons(moduleId);
            rptLessons.DataSource = lessons;
            rptLessons.DataBind();
        }

        private ModuleItem GetModuleById(int id)
        {
            var modules = new List<ModuleItem>
            {
                new ModuleItem { ModuleID = 1, Title = "Introduction to Emergency Response" },
                new ModuleItem { ModuleID = 2, Title = "CPR and First Aid" }
            };
            return modules.Find(m => m.ModuleID == id);
        }

        private List<LessonItem> GetLessons(int moduleId)
        {
            return new List<LessonItem>
            {
                new LessonItem { LessonID = 1, Title = "What is Emergency Response?", LessonOrder = 1, IsActive = true, HasVideo = true, HasResources = false },
                new LessonItem { LessonID = 2, Title = "Assessment of Emergency Situations", LessonOrder = 2, IsActive = true, HasVideo = false, HasResources = true },
                new LessonItem { LessonID = 3, Title = "Emergency Response Plans", LessonOrder = 3, IsActive = false, HasVideo = false, HasResources = false }
            };
        }

        protected void rptLessons_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int lessonId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Edit":
                    EditLesson(lessonId);
                    break;
                case "Toggle":
                    ToggleLesson(lessonId);
                    break;
                case "Delete":
                    DeleteLesson(lessonId);
                    break;
            }
        }

        private void EditLesson(int lessonId)
        {
            var lesson = GetLessonById(lessonId);
            if (lesson != null)
            {
                lblModalTitle.Text = "Edit Lesson";
                hfLessonID.Value = lessonId.ToString();
                txtTitle.Text = lesson.Title;
                txtContent.Text = lesson.Content ?? "";
                txtOrder.Text = lesson.LessonOrder.ToString();
                txtVideoUrl.Text = lesson.VideoUrl;
                txtResourceUrl.Text = lesson.ResourceUrl;
                chkIsActive.Checked = lesson.IsActive;

                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalLesson').modal('show');", true);
            }
        }

        private void ToggleLesson(int lessonId)
        {
            ShowSuccess("Lesson status updated successfully");
            LoadLessons();
        }

        private void DeleteLesson(int lessonId)
        {
            ShowSuccess("Lesson deleted successfully");
            LoadLessons();
        }

        protected void btnSaveLesson_Click(object sender, EventArgs e)
        {
            int lessonId;
            bool isNew = !int.TryParse(hfLessonID.Value, out lessonId) || lessonId == 0;

            if (string.IsNullOrEmpty(txtTitle.Text) || string.IsNullOrEmpty(txtContent.Text))
            {
                ShowError("Title and content are required.");
                return;
            }

            ShowSuccess(isNew ? "Lesson added successfully" : "Lesson updated successfully");
            ClearForm();
            LoadLessons();

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalLesson').modal('hide');", true);
        }

        private LessonItem GetLessonById(int lessonId)
        {
            var lessons = GetLessons(moduleId);
            return lessons.Find(l => l.LessonID == lessonId);
        }

        private void ClearForm()
        {
            hfLessonID.Value = "";
            txtTitle.Text = "";
            txtContent.Text = "";
            txtOrder.Text = "";
            txtVideoUrl.Text = "";
            txtResourceUrl.Text = "";
            chkIsActive.Checked = true;
            lblModalTitle.Text = "Add Lesson";
        }

        protected void btnAddLesson_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalLesson').modal('show');", true);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageModules.aspx");
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