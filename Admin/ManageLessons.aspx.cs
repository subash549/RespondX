using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageLessons : Page
    {
        private static string connectionString => DatabaseHelper.ConnectionString;

        private int moduleId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!int.TryParse(Request.QueryString["moduleId"], out moduleId) || moduleId <= 0)
            {
                RedirectToModules();
                return;
            }

            var module = GetModuleById(moduleId);
            if (module == null)
            {
                RedirectToModules();
                return;
            }

            hfModuleID.Value = moduleId.ToString();
            if (!IsPostBack)
            {
                lblModuleInfo.Text = "Module: " + Server.HtmlEncode(module.Title);
                LoadLessons();
            }
        }

        private void RedirectToModules()
        {
            Response.Redirect("ManageModules.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private ModuleItem GetModuleById(int id)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(
                "SELECT ModuleID, Title FROM dbo.Modules WHERE ModuleID = @ModuleID;", conn))
            {
                cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = id;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new ModuleItem
                    {
                        ModuleID = Convert.ToInt32(reader["ModuleID"]),
                        Title = Convert.ToString(reader["Title"])
                    };
                }
            }
        }

        private void LoadLessons()
        {
            rptLessons.DataSource = GetLessons(moduleId);
            rptLessons.DataBind();
        }

        private List<LessonItem> GetLessons(int selectedModuleId)
        {
            var lessons = new List<LessonItem>();
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"
                SELECT LessonID, ModuleID, Title, Content, LessonOrder, IsActive, VideoUrl, ResourceUrl
                FROM dbo.Lessons
                WHERE ModuleID = @ModuleID
                ORDER BY LessonOrder, LessonID;", conn))
            {
                cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = selectedModuleId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string videoUrl = reader["VideoUrl"] == DBNull.Value ? string.Empty : Convert.ToString(reader["VideoUrl"]);
                        string resourceUrl = reader["ResourceUrl"] == DBNull.Value ? string.Empty : Convert.ToString(reader["ResourceUrl"]);
                        lessons.Add(new LessonItem
                        {
                            LessonID = Convert.ToInt32(reader["LessonID"]),
                            ModuleId = Convert.ToInt32(reader["ModuleID"]),
                            Title = Convert.ToString(reader["Title"]),
                            Content = reader["Content"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Content"]),
                            LessonOrder = Convert.ToInt32(reader["LessonOrder"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            VideoUrl = videoUrl,
                            ResourceUrl = resourceUrl,
                            HasVideo = !string.IsNullOrWhiteSpace(videoUrl),
                            HasResources = !string.IsNullOrWhiteSpace(resourceUrl)
                        });
                    }
                }
            }

            return lessons;
        }

        protected void rptLessons_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int lessonId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out lessonId) || lessonId <= 0)
            {
                ShowError("The selected lesson could not be found.");
                return;
            }

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
            if (lesson == null)
            {
                ShowError("This lesson does not belong to the selected module.");
                LoadLessons();
                return;
            }

            lblModalTitle.Text = "Edit Lesson";
            pnlModalError.Visible = false;
            lblModalError.Text = string.Empty;
            hfLessonID.Value = lessonId.ToString();
            txtTitle.Text = lesson.Title;
            txtContent.Text = lesson.Content;
            txtOrder.Text = lesson.LessonOrder.ToString();
            txtVideoUrl.Text = lesson.VideoUrl;
            txtResourceUrl.Text = lesson.ResourceUrl;
            chkIsActive.Checked = lesson.IsActive;
            OpenLessonModal();
        }

        private void ToggleLesson(int lessonId)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"
                    UPDATE dbo.Lessons
                    SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END,
                        UpdatedAt = GETDATE()
                    WHERE LessonID = @LessonID AND ModuleID = @ModuleID;", conn))
                {
                    cmd.Parameters.Add("@LessonID", SqlDbType.Int).Value = lessonId;
                    cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId;
                    conn.Open();
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        ShowError("This lesson does not belong to the selected module.");
                        LoadLessons();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Toggle lesson status", ex.Message, ex.StackTrace);
                ShowError("Unable to update this lesson status. Please try again.");
                LoadLessons();
                return;
            }

            ShowSuccess("Lesson status updated successfully.");
            LoadLessons();
        }

        private void DeleteLesson(int lessonId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (var progressCommand = new SqlCommand(
                            "DELETE FROM dbo.LearnerProgress WHERE LessonID = @LessonID AND ModuleID = @ModuleID;",
                            conn, transaction))
                        {
                            progressCommand.Parameters.Add("@LessonID", SqlDbType.Int).Value = lessonId;
                            progressCommand.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId;
                            progressCommand.ExecuteNonQuery();
                        }

                        int deleted;
                        using (var lessonCommand = new SqlCommand(
                            "DELETE FROM dbo.Lessons WHERE LessonID = @LessonID AND ModuleID = @ModuleID;",
                            conn, transaction))
                        {
                            lessonCommand.Parameters.Add("@LessonID", SqlDbType.Int).Value = lessonId;
                            lessonCommand.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId;
                            deleted = lessonCommand.ExecuteNonQuery();
                        }

                        if (deleted == 0)
                        {
                            transaction.Rollback();
                            ShowError("This lesson does not belong to the selected module.");
                            LoadLessons();
                            return;
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        DatabaseHelper.LogError("Delete lesson", ex.Message, ex.StackTrace);
                        ShowError("Unable to delete this lesson. Please try again.");
                        LoadLessons();
                        return;
                    }
                }
            }

            ShowSuccess("Lesson and its learner progress were deleted successfully.");
            LoadLessons();
        }

        protected void btnSaveLesson_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string content = txtContent.Text.Trim();
            int lessonId;
            bool isNew = !int.TryParse(hfLessonID.Value, out lessonId) || lessonId <= 0;

            if (string.IsNullOrWhiteSpace(title) || title.Length < 3 || title.Length > 100 || string.IsNullOrWhiteSpace(content))
            {
                ShowFormError("Enter a lesson title between 3 and 100 characters and lesson content.");
                return;
            }

            int lessonOrder;
            if (string.IsNullOrWhiteSpace(txtOrder.Text))
            {
                lessonOrder = GetNextLessonOrder();
            }
            else if (!int.TryParse(txtOrder.Text.Trim(), out lessonOrder) || lessonOrder < 1)
            {
                ShowFormError("Lesson order must be a whole number of 1 or higher.");
                return;
            }

            string videoUrl = txtVideoUrl.Text.Trim();
            string resourceUrl = txtResourceUrl.Text.Trim();
            if (!IsOptionalWebUrl(videoUrl) || !IsOptionalWebUrl(resourceUrl))
            {
                ShowFormError("Video and resource links must be valid HTTP or HTTPS URLs.");
                return;
            }

            if (!isNew && GetLessonById(lessonId) == null)
            {
                ShowError("This lesson does not belong to the selected module.");
                ClearForm();
                LoadLessons();
                return;
            }

            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(isNew ? @"
                    INSERT INTO dbo.Lessons
                        (ModuleID, Title, Content, LessonOrder, IsActive, CreatedAt, VideoUrl, ResourceUrl)
                    VALUES
                        (@ModuleID, @Title, @Content, @LessonOrder, @IsActive, GETDATE(), @VideoUrl, @ResourceUrl);" : @"
                    UPDATE dbo.Lessons
                    SET Title = @Title,
                        Content = @Content,
                        LessonOrder = @LessonOrder,
                        IsActive = @IsActive,
                        VideoUrl = @VideoUrl,
                        ResourceUrl = @ResourceUrl,
                        UpdatedAt = GETDATE()
                    WHERE LessonID = @LessonID AND ModuleID = @ModuleID;", conn))
                {
                    AddLessonParameters(cmd, title, content, lessonOrder, videoUrl, resourceUrl);
                    cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId;
                    if (!isNew)
                        cmd.Parameters.Add("@LessonID", SqlDbType.Int).Value = lessonId;

                    conn.Open();
                    int saved = cmd.ExecuteNonQuery();
                    if (saved == 0)
                    {
                        ShowFormError("This lesson could not be saved in the selected module.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Save lesson", ex.Message, ex.StackTrace);
                ShowFormError("Unable to save this lesson. Please review the fields and try again.");
                return;
            }

            ShowSuccess(isNew ? "Lesson added successfully." : "Lesson updated successfully.");
            try { NotificationRepository.NotifyRoles("Content", isNew ? "New lesson available" : "Lesson updated", title, "~/Learner/Modules.aspx", SessionHelper.GetCurrentUserId(), "Learner", "Expert"); }
            catch (Exception notificationError) { DatabaseHelper.LogError("Lesson notification", notificationError.Message, notificationError.StackTrace); }
            ClearForm();
            pnlLessonEditor.Visible = false;
            LoadLessons();
        }

        private void AddLessonParameters(SqlCommand cmd, string title, string content, int lessonOrder,
            string videoUrl, string resourceUrl)
        {
            cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = title;
            cmd.Parameters.Add("@Content", SqlDbType.NVarChar, -1).Value = content;
            cmd.Parameters.Add("@LessonOrder", SqlDbType.Int).Value = lessonOrder;
            cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = chkIsActive.Checked;
            cmd.Parameters.Add("@VideoUrl", SqlDbType.NVarChar, 1000).Value =
                string.IsNullOrWhiteSpace(videoUrl) ? (object)DBNull.Value : videoUrl;
            cmd.Parameters.Add("@ResourceUrl", SqlDbType.NVarChar, 1000).Value =
                string.IsNullOrWhiteSpace(resourceUrl) ? (object)DBNull.Value : resourceUrl;
        }

        private int GetNextLessonOrder()
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(
                "SELECT ISNULL(MAX(LessonOrder), 0) + 1 FROM dbo.Lessons WHERE ModuleID = @ModuleID;", conn))
            {
                cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId;
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private static bool IsOptionalWebUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            Uri uri;
            return Uri.TryCreate(value, UriKind.Absolute, out uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        private LessonItem GetLessonById(int lessonId)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"
                SELECT LessonID, ModuleID, Title, Content, LessonOrder, IsActive, VideoUrl, ResourceUrl
                FROM dbo.Lessons
                WHERE LessonID = @LessonID AND ModuleID = @ModuleID;", conn))
            {
                cmd.Parameters.Add("@LessonID", SqlDbType.Int).Value = lessonId;
                cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new LessonItem
                    {
                        LessonID = Convert.ToInt32(reader["LessonID"]),
                        ModuleId = Convert.ToInt32(reader["ModuleID"]),
                        Title = Convert.ToString(reader["Title"]),
                        Content = reader["Content"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Content"]),
                        LessonOrder = Convert.ToInt32(reader["LessonOrder"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        VideoUrl = reader["VideoUrl"] == DBNull.Value ? string.Empty : Convert.ToString(reader["VideoUrl"]),
                        ResourceUrl = reader["ResourceUrl"] == DBNull.Value ? string.Empty : Convert.ToString(reader["ResourceUrl"])
                    };
                }
            }
        }

        private void ClearForm()
        {
            hfLessonID.Value = string.Empty;
            txtTitle.Text = string.Empty;
            txtContent.Text = string.Empty;
            txtOrder.Text = string.Empty;
            txtVideoUrl.Text = string.Empty;
            txtResourceUrl.Text = string.Empty;
            chkIsActive.Checked = true;
            lblModalTitle.Text = "Add Lesson";
            pnlModalError.Visible = false;
            lblModalError.Text = string.Empty;
        }

        protected void btnAddLesson_Click(object sender, EventArgs e)
        {
            ClearForm();
            OpenLessonModal();
        }

        private void OpenLessonModal()
        {
            pnlLessonEditor.Visible = true;
        }

        protected void btnCancelLesson_Click(object sender, EventArgs e)
        {
            ClearForm();
            pnlLessonEditor.Visible = false;
            pnlError.Visible = false;
        }

        private void ShowFormError(string message)
        {
            ShowError(message);
            pnlModalError.Visible = true;
            lblModalError.Text = message;
            OpenLessonModal();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            RedirectToModules();
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
