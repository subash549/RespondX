using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageModules : Page
    {
        private const int SqlForeignKeyViolation = 547;
        private const string DefaultThumbnail = "~/Content/Images/default-module.svg";

        private static string connString => DatabaseHelper.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            Page.Form.Enctype = "multipart/form-data";

            if (!IsPostBack)
            {
                LoadDropdowns();
                LoadModules();
            }
        }

        protected string GetThumbnail(object thumbnailUrl)
        {
            string url = Convert.ToString(thumbnailUrl);
            if (string.IsNullOrWhiteSpace(url))
                return DefaultThumbnail;

            Uri uri;
            bool isWebUrl = Uri.TryCreate(url, UriKind.Absolute, out uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
            return isWebUrl || url.StartsWith("~/") || url.StartsWith("/") ? url : DefaultThumbnail;
        }

        private void LoadDropdowns()
        {
            ddlCategory.Items.Clear();
            ddlCategory.Items.Add(new ListItem("-- Select Category --", ""));
            ddlInstructor.Items.Clear();
            ddlInstructor.Items.Add(new ListItem("-- Select Instructor --", ""));

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT CategoryID, Name FROM dbo.Categories WHERE IsActive = 1 ORDER BY Name;", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ddlCategory.Items.Add(new ListItem(reader["Name"].ToString(), reader["CategoryID"].ToString()));
                    }
                }

                using (var cmd = new SqlCommand(@"
                    SELECT UserID, FirstName + N' ' + LastName AS FullName
                    FROM dbo.Users WHERE Role = N'Expert' AND IsActive = 1
                    ORDER BY FirstName, LastName;", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ddlInstructor.Items.Add(new ListItem(reader["FullName"].ToString(), reader["UserID"].ToString()));
                    }
                }
            }
        }

        private void LoadModules()
        {
            var modules = GetModulesFromDB(null);

            var status = ddlStatus.SelectedValue;
            if (status != "All")
            {
                bool isActive = status == "Active";
                modules = modules.FindAll(m => m.IsActive == isActive);
            }

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                modules = modules.FindAll(m =>
                    m.Title.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    m.Description.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    m.CategoryName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            rptModules.DataSource = modules;
            rptModules.DataBind();
            pnlNoModules.Visible = modules.Count == 0;
        }

        private List<ModuleItem> GetModulesFromDB(int? moduleId)
        {
            var modules = new List<ModuleItem>();
            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(@"
                SELECT m.ModuleID, m.Title, m.Description, m.ModuleOrder, m.IsActive, m.CreatedAt,
                       m.EstimatedHours, m.IsMandatory, m.CategoryID, c.Name AS CategoryName,
                       m.ThumbnailUrl, m.YouTubeVideoUrl, m.VideoFileUrl, m.InstructorID,
                       u.FirstName + N' ' + u.LastName AS InstructorName,
                       (SELECT COUNT(*) FROM dbo.Lessons l WHERE l.ModuleID = m.ModuleID) AS LessonCount
                FROM dbo.Modules m
                LEFT JOIN dbo.Categories c ON m.CategoryID = c.CategoryID
                LEFT JOIN dbo.Users u ON m.InstructorID = u.UserID
                WHERE @ModuleID IS NULL OR m.ModuleID = @ModuleID
                ORDER BY m.ModuleOrder, m.ModuleID;", conn))
            {
                cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId.HasValue ? (object)moduleId.Value : DBNull.Value;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        modules.Add(new ModuleItem
                        {
                            ModuleID = Convert.ToInt32(reader["ModuleID"]),
                            Title = Convert.ToString(reader["Title"]),
                            Description = Convert.ToString(reader["Description"]),
                            ModuleOrder = reader["ModuleOrder"] != DBNull.Value ? Convert.ToInt32(reader["ModuleOrder"]) : 1,
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.Now,
                            EstimatedHours = reader["EstimatedHours"] != DBNull.Value ? Convert.ToInt32(reader["EstimatedHours"]) : 0,
                            IsMandatory = reader["IsMandatory"] != DBNull.Value && Convert.ToBoolean(reader["IsMandatory"]),
                            CategoryID = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0,
                            CategoryName = reader["CategoryName"] != DBNull.Value ? Convert.ToString(reader["CategoryName"]) : "Uncategorized",
                            ThumbnailUrl = Convert.ToString(reader["ThumbnailUrl"]),
                            YouTubeVideoUrl = Convert.ToString(reader["YouTubeVideoUrl"]),
                            VideoFileUrl = Convert.ToString(reader["VideoFileUrl"]),
                            InstructorID = reader["InstructorID"] != DBNull.Value ? Convert.ToInt32(reader["InstructorID"]) : 0,
                            InstructorName = Convert.ToString(reader["InstructorName"]),
                            LessonCount = Convert.ToInt32(reader["LessonCount"])
                        });
                    }
                }
            }
            return modules;
        }

        protected void rptModules_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int moduleId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out moduleId))
                return;

            switch (e.CommandName)
            {
                case "EditModule":
                    EditModule(moduleId);
                    break;
                case "Lessons":
                    Redirect("ManageLessons.aspx?moduleId=" + moduleId);
                    break;
                case "Quizzes":
                    Redirect("ManageQuizzes.aspx?moduleId=" + moduleId);
                    break;
                case "Scenarios":
                    Redirect("ManageScenarios.aspx?moduleId=" + moduleId);
                    break;
                case "ToggleModule":
                    ToggleModule(moduleId);
                    break;
                case "DeleteModule":
                    DeleteModule(moduleId);
                    break;
            }
        }

        private void Redirect(string url)
        {
            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void EditModule(int moduleId)
        {
            var modules = GetModulesFromDB(moduleId);
            if (modules.Count == 0)
            {
                ShowError("That module no longer exists.");
                LoadModules();
                return;
            }

            var module = modules[0];
            ClearForm();
            lblModalTitle.Text = "Edit Module";
            hfModuleID.Value = moduleId.ToString();
            txtTitle.Text = module.Title;
            txtDescription.Text = module.Description;
            txtOrder.Text = module.ModuleOrder.ToString();
            txtHours.Text = module.EstimatedHours.ToString();
            chkIsMandatory.Checked = module.IsMandatory;
            chkIsActive.Checked = module.IsActive;
            SelectOrAdd(ddlCategory, module.CategoryID, module.CategoryName + " (inactive)");
            SelectOrAdd(ddlInstructor, module.InstructorID, module.InstructorName + " (inactive)");
            txtThumbnailUrl.Text = module.ThumbnailUrl;
            txtYouTubeVideoUrl.Text = module.YouTubeVideoUrl;
            if (!string.IsNullOrEmpty(module.VideoFileUrl))
            {
                lblCurrentVideo.Visible = true;
                lblCurrentVideo.Text = "Current uploaded video: " + Server.HtmlEncode(module.VideoFileUrl);
            }

            UiHelper.ShowModal(this, "modalModule");
        }

        // A module can reference a category or instructor that has since been deactivated;
        // keep that value selectable instead of throwing when setting SelectedValue.
        private static void SelectOrAdd(DropDownList list, int id, string fallbackText)
        {
            if (id <= 0)
            {
                list.SelectedIndex = 0;
                return;
            }

            string value = id.ToString();
            if (list.Items.FindByValue(value) == null)
                list.Items.Add(new ListItem(fallbackText, value));
            list.SelectedValue = value;
        }

        private void ToggleModule(int moduleId)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                using (var cmd = new SqlCommand(@"
                    UPDATE dbo.Modules
                    SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END, UpdatedAt = GETDATE()
                    WHERE ModuleID = @ID;", conn))
                {
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = moduleId;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                ShowSuccess("Module status updated.");
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Toggle module", ex.Message, ex.StackTrace);
                ShowError("Unable to update this module. Please try again.");
            }
            LoadModules();
        }

        private void DeleteModule(int moduleId)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                using (var cmd = new SqlCommand("DELETE FROM dbo.Modules WHERE ModuleID = @ID;", conn))
                {
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = moduleId;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                ShowSuccess("Module deleted.");
            }
            catch (SqlException ex) when (ex.Number == SqlForeignKeyViolation)
            {
                ShowError("This module still has lessons, quizzes, scenarios or learner activity. Deactivate it instead, or remove its content first.");
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Delete module", ex.Message, ex.StackTrace);
                ShowError("Unable to delete this module. Please try again.");
            }
            LoadModules();
        }

        protected void btnSaveModule_Click(object sender, EventArgs e)
        {
            int moduleId;
            bool isNew = !int.TryParse(hfModuleID.Value, out moduleId) || moduleId <= 0;

            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            string thumbnailUrl = txtThumbnailUrl.Text.Trim();
            string youTubeUrl = txtYouTubeVideoUrl.Text.Trim();
            int moduleOrder = 1;
            int hours = 0;

            if (title.Length == 0 || title.Length > 100)
            {
                ShowFormError("Enter a module title of up to 100 characters.");
                return;
            }
            if (description.Length > 500)
            {
                ShowFormError("The description must be 500 characters or fewer.");
                return;
            }
            if (txtOrder.Text.Trim().Length > 0 && (!int.TryParse(txtOrder.Text.Trim(), out moduleOrder) || moduleOrder < 1))
            {
                ShowFormError("Module order must be a whole number of 1 or higher.");
                return;
            }
            if (txtHours.Text.Trim().Length > 0 && (!int.TryParse(txtHours.Text.Trim(), out hours) || hours < 0))
            {
                ShowFormError("Estimated hours must be a whole number of 0 or higher.");
                return;
            }
            if (youTubeUrl.Length > 0 && ModuleMediaHelper.GetYouTubeEmbedUrl(youTubeUrl) == null)
            {
                ShowFormError("That doesn't look like a YouTube video link.");
                return;
            }

            int categoryId, instructorId;
            bool hasCategory = int.TryParse(ddlCategory.SelectedValue, out categoryId);
            bool hasInstructor = int.TryParse(ddlInstructor.SelectedValue, out instructorId);

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    using (var cmd = new SqlCommand(isNew
                        ? @"INSERT INTO dbo.Modules (Title, Description, ModuleOrder, IsActive, CreatedAt, EstimatedHours, IsMandatory, CategoryID, ThumbnailUrl, YouTubeVideoUrl, InstructorID)
                            VALUES (@Title, @Description, @ModuleOrder, @IsActive, GETDATE(), @EstimatedHours, @IsMandatory, @CategoryID, @ThumbnailUrl, @YouTubeVideoUrl, @InstructorID);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);"
                        : @"UPDATE dbo.Modules
                            SET Title = @Title, Description = @Description, ModuleOrder = @ModuleOrder, IsActive = @IsActive,
                                EstimatedHours = @EstimatedHours, IsMandatory = @IsMandatory, CategoryID = @CategoryID,
                                ThumbnailUrl = @ThumbnailUrl, YouTubeVideoUrl = @YouTubeVideoUrl, InstructorID = @InstructorID,
                                UpdatedAt = GETDATE()
                            WHERE ModuleID = @ID;
                            SELECT @ID;", conn))
                    {
                        cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = title;
                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = description.Length == 0 ? (object)DBNull.Value : description;
                        cmd.Parameters.Add("@ModuleOrder", SqlDbType.Int).Value = moduleOrder;
                        cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = chkIsActive.Checked;
                        cmd.Parameters.Add("@EstimatedHours", SqlDbType.Int).Value = hours;
                        cmd.Parameters.Add("@IsMandatory", SqlDbType.Bit).Value = chkIsMandatory.Checked;
                        cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = hasCategory ? (object)categoryId : DBNull.Value;
                        cmd.Parameters.Add("@ThumbnailUrl", SqlDbType.NVarChar, 500).Value = thumbnailUrl.Length == 0 ? (object)DBNull.Value : thumbnailUrl;
                        cmd.Parameters.Add("@YouTubeVideoUrl", SqlDbType.NVarChar, 500).Value = youTubeUrl.Length == 0 ? (object)DBNull.Value : youTubeUrl;
                        cmd.Parameters.Add("@InstructorID", SqlDbType.Int).Value = hasInstructor ? (object)instructorId : DBNull.Value;
                        if (!isNew)
                            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = moduleId;

                        moduleId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (fuModuleVideo.HasFile)
                    {
                        string videoFileUrl;
                        try
                        {
                            videoFileUrl = ModuleMediaHelper.SaveUploadedVideo(fuModuleVideo.PostedFile, moduleId);
                        }
                        catch (InvalidOperationException ex)
                        {
                            // The module itself was saved; only the video was rejected.
                            ShowError("Module saved, but the video was not uploaded: " + ex.Message);
                            ClearForm();
                            LoadModules();
                            return;
                        }

                        using (var updateVideo = new SqlCommand("UPDATE dbo.Modules SET VideoFileUrl = @VideoFileUrl WHERE ModuleID = @ID;", conn))
                        {
                            updateVideo.Parameters.Add("@VideoFileUrl", SqlDbType.NVarChar, 500).Value = videoFileUrl;
                            updateVideo.Parameters.Add("@ID", SqlDbType.Int).Value = moduleId;
                            updateVideo.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Save module", ex.Message, ex.StackTrace);
                ShowFormError("Unable to save this module. Please try again.");
                return;
            }

            ShowSuccess(isNew ? "Module added." : "Module updated.");
            ClearForm();
            LoadModules();
            UiHelper.HideModal(this, "modalModule");
        }

        private void ClearForm()
        {
            hfModuleID.Value = string.Empty;
            txtTitle.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtOrder.Text = string.Empty;
            txtHours.Text = string.Empty;
            chkIsMandatory.Checked = false;
            chkIsActive.Checked = true;
            ddlCategory.SelectedIndex = 0;
            ddlInstructor.SelectedIndex = 0;
            txtThumbnailUrl.Text = string.Empty;
            txtYouTubeVideoUrl.Text = string.Empty;
            lblCurrentVideo.Visible = false;
            lblCurrentVideo.Text = string.Empty;
            lblModalTitle.Text = "Add Module";
            pnlModalError.Visible = false;
        }

        protected void btnAddModule_Click(object sender, EventArgs e)
        {
            ClearForm();
            UiHelper.ShowModal(this, "modalModule");
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadModules();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadModules();
        }

        private void ShowFormError(string message)
        {
            pnlModalError.Visible = true;
            lblModalError.Text = Server.HtmlEncode(message);
            UiHelper.ShowModal(this, "modalModule");
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            lblSuccess.Text = Server.HtmlEncode(message);
            pnlError.Visible = false;
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = Server.HtmlEncode(message);
            pnlSuccess.Visible = false;
        }
    }
}
