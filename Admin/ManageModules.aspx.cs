using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageModules : Page
    {
        private string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString ?? "Data Source=DESKTOP-5UH7Q5H\\SQLEXPRESS01;Initial Catalog=RespondX;Integrated Security=True;TrustServerCertificate=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            Page.Form.Enctype = "multipart/form-data";
            ModuleMediaHelper.EnsureModuleMediaColumns();

            if (!IsPostBack)
            {
                LoadDropdowns();
                LoadModules();
            }
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
                using (var cmd = new SqlCommand("SELECT CategoryID, Name FROM Categories WHERE IsActive = 1", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ddlCategory.Items.Add(new ListItem(reader["Name"].ToString(), reader["CategoryID"].ToString()));
                    }
                }
                
                using (var cmd = new SqlCommand("SELECT UserID, FirstName + ' ' + LastName AS FullName FROM Users WHERE Role = 'Expert' AND IsActive = 1", conn))
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
            var modules = GetModulesFromDB();

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
                    m.Title.ToLower().Contains(search.ToLower()) ||
                    m.Description.ToLower().Contains(search.ToLower())
                );
            }

            rptModules.DataSource = modules;
            rptModules.DataBind();
        }

        private List<ModuleItem> GetModulesFromDB()
        {
            var modules = new List<ModuleItem>();
            using (var conn = new SqlConnection(connString))
            {
                var query = @"
                    SELECT m.ModuleID, m.Title, m.Description, m.ModuleOrder, m.IsActive, m.CreatedAt, 
                           m.EstimatedHours, m.IsMandatory, m.CategoryID, c.Name AS CategoryName, 
                           m.ThumbnailUrl, m.YouTubeVideoUrl, m.VideoFileUrl, m.InstructorID, u.FirstName + ' ' + u.LastName AS InstructorName,
                           (SELECT COUNT(*) FROM Lessons l WHERE l.ModuleID = m.ModuleID) AS LessonCount
                    FROM Modules m
                    LEFT JOIN Categories c ON m.CategoryID = c.CategoryID
                    LEFT JOIN Users u ON m.InstructorID = u.UserID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            modules.Add(new ModuleItem
                            {
                                ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                ModuleOrder = reader["ModuleOrder"] != DBNull.Value ? Convert.ToInt32(reader["ModuleOrder"]) : 0,
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.Now,
                                EstimatedHours = reader["EstimatedHours"] != DBNull.Value ? Convert.ToInt32(reader["EstimatedHours"]) : 0,
                                IsMandatory = reader["IsMandatory"] != DBNull.Value ? Convert.ToBoolean(reader["IsMandatory"]) : false,
                                CategoryID = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0,
                                CategoryName = reader["CategoryName"].ToString(),
                                ThumbnailUrl = reader["ThumbnailUrl"] != DBNull.Value ? reader["ThumbnailUrl"].ToString() : "",
                                YouTubeVideoUrl = reader["YouTubeVideoUrl"] != DBNull.Value ? reader["YouTubeVideoUrl"].ToString() : "",
                                VideoFileUrl = reader["VideoFileUrl"] != DBNull.Value ? reader["VideoFileUrl"].ToString() : "",
                                InstructorID = reader["InstructorID"] != DBNull.Value ? Convert.ToInt32(reader["InstructorID"]) : 0,
                                InstructorName = reader["InstructorName"].ToString(),
                                LessonCount = Convert.ToInt32(reader["LessonCount"])
                            });
                        }
                    }
                }
            }
            return modules;
        }

        protected void rptModules_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int moduleId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Edit":
                    EditModule(moduleId);
                    break;
                case "Lessons":
                    Response.Redirect($"ManageLessons.aspx?moduleId={moduleId}");
                    break;
                case "Toggle":
                    ToggleModule(moduleId);
                    break;
                case "Delete":
                    DeleteModule(moduleId);
                    break;
            }
        }

        private void EditModule(int moduleId)
        {
            var module = GetModulesFromDB().Find(m => m.ModuleID == moduleId);
            if (module != null)
            {
                lblModalTitle.Text = "Edit Module";
                hfModuleID.Value = moduleId.ToString();
                txtTitle.Text = module.Title;
                txtDescription.Text = module.Description;
                txtOrder.Text = module.ModuleOrder.ToString();
                txtHours.Text = module.EstimatedHours.ToString();
                chkIsMandatory.Checked = module.IsMandatory;
                chkIsActive.Checked = module.IsActive;
                ddlCategory.SelectedValue = module.CategoryID > 0 ? module.CategoryID.ToString() : "";
                ddlInstructor.SelectedValue = module.InstructorID > 0 ? module.InstructorID.ToString() : "";
                txtThumbnailUrl.Text = module.ThumbnailUrl;
                txtYouTubeVideoUrl.Text = module.YouTubeVideoUrl;
                if (!string.IsNullOrEmpty(module.VideoFileUrl))
                {
                    lblCurrentVideo.Visible = true;
                    lblCurrentVideo.Text = "Current uploaded video: " + module.VideoFileUrl;
                }
                else
                {
                    lblCurrentVideo.Visible = false;
                    lblCurrentVideo.Text = "";
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalModule').modal('show');", true);
            }
        }

        private void ToggleModule(int moduleId)
        {
            using (var conn = new SqlConnection(connString))
            {
                using (var cmd = new SqlCommand("UPDATE Modules SET IsActive = ~IsActive WHERE ModuleID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", moduleId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            ShowSuccess("Module status updated successfully");
            LoadModules();
        }

        private void DeleteModule(int moduleId)
        {
            using (var conn = new SqlConnection(connString))
            {
                using (var cmd = new SqlCommand("DELETE FROM Modules WHERE ModuleID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", moduleId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            ShowSuccess("Module deleted successfully");
            LoadModules();
        }

        protected void btnSaveModule_Click(object sender, EventArgs e)
        {
            int moduleId;
            bool isNew = !int.TryParse(hfModuleID.Value, out moduleId) || moduleId == 0;

            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                ShowError("Title is required.");
                return;
            }

            string videoFileUrl = null;
            if (!isNew)
            {
                var existing = GetModulesFromDB().Find(m => m.ModuleID == moduleId);
                videoFileUrl = existing?.VideoFileUrl;
            }

            using (var conn = new SqlConnection(connString))
            {
                string query = isNew ? 
                    @"INSERT INTO Modules (Title, Description, ModuleOrder, IsActive, CreatedAt, EstimatedHours, IsMandatory, CategoryID, ThumbnailUrl, YouTubeVideoUrl, VideoFileUrl, InstructorID) 
                      VALUES (@Title, @Description, @ModuleOrder, @IsActive, GETDATE(), @EstimatedHours, @IsMandatory, @CategoryID, @ThumbnailUrl, @YouTubeVideoUrl, @VideoFileUrl, @InstructorID)" : 
                    @"UPDATE Modules SET Title = @Title, Description = @Description, ModuleOrder = @ModuleOrder, IsActive = @IsActive, 
                             EstimatedHours = @EstimatedHours, IsMandatory = @IsMandatory, CategoryID = @CategoryID, 
                             ThumbnailUrl = @ThumbnailUrl, YouTubeVideoUrl = @YouTubeVideoUrl, VideoFileUrl = @VideoFileUrl, InstructorID = @InstructorID 
                      WHERE ModuleID = @ID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@ModuleOrder", string.IsNullOrEmpty(txtOrder.Text) ? 1 : int.Parse(txtOrder.Text));
                    cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);
                    cmd.Parameters.AddWithValue("@EstimatedHours", string.IsNullOrEmpty(txtHours.Text) ? 0 : int.Parse(txtHours.Text));
                    cmd.Parameters.AddWithValue("@IsMandatory", chkIsMandatory.Checked);
                    
                    cmd.Parameters.AddWithValue("@CategoryID", string.IsNullOrEmpty(ddlCategory.SelectedValue) ? (object)DBNull.Value : int.Parse(ddlCategory.SelectedValue));
                    cmd.Parameters.AddWithValue("@ThumbnailUrl", string.IsNullOrEmpty(txtThumbnailUrl.Text) ? (object)DBNull.Value : txtThumbnailUrl.Text.Trim());
                    cmd.Parameters.AddWithValue("@YouTubeVideoUrl", string.IsNullOrEmpty(txtYouTubeVideoUrl.Text) ? (object)DBNull.Value : txtYouTubeVideoUrl.Text.Trim());
                    cmd.Parameters.AddWithValue("@VideoFileUrl", string.IsNullOrEmpty(videoFileUrl) ? (object)DBNull.Value : videoFileUrl);
                    cmd.Parameters.AddWithValue("@InstructorID", string.IsNullOrEmpty(ddlInstructor.SelectedValue) ? (object)DBNull.Value : int.Parse(ddlInstructor.SelectedValue));

                    if (!isNew)
                    {
                        cmd.Parameters.AddWithValue("@ID", moduleId);
                    }
                    
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (isNew)
                    {
                        using (var idCmd = new SqlCommand("SELECT CAST(SCOPE_IDENTITY() AS INT)", conn))
                        {
                            moduleId = (int)idCmd.ExecuteScalar();
                        }
                    }
                }

                if (fuModuleVideo.HasFile)
                {
                    try
                    {
                        videoFileUrl = ModuleMediaHelper.SaveUploadedVideo(fuModuleVideo.PostedFile, moduleId);
                        using (var updateVideo = new SqlCommand("UPDATE Modules SET VideoFileUrl = @VideoFileUrl WHERE ModuleID = @ID", conn))
                        {
                            updateVideo.Parameters.AddWithValue("@VideoFileUrl", videoFileUrl);
                            updateVideo.Parameters.AddWithValue("@ID", moduleId);
                            updateVideo.ExecuteNonQuery();
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        ShowError(ex.Message);
                        return;
                    }
                }
            }

            ShowSuccess(isNew ? "Module added successfully" : "Module updated successfully");
            ClearForm();
            LoadModules();

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalModule').modal('hide');", true);
        }

        private void ClearForm()
        {
            hfModuleID.Value = "";
            txtTitle.Text = "";
            txtDescription.Text = "";
            txtOrder.Text = "";
            txtHours.Text = "";
            chkIsMandatory.Checked = false;
            chkIsActive.Checked = true;
            ddlCategory.SelectedIndex = 0;
            ddlInstructor.SelectedIndex = 0;
            txtThumbnailUrl.Text = "";
            txtYouTubeVideoUrl.Text = "";
            lblCurrentVideo.Visible = false;
            lblCurrentVideo.Text = "";
            lblModalTitle.Text = "Add Module";
        }

        protected void btnAddModule_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalModule').modal('show');", true);
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadModules();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadModules();
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