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
    public partial class ManageCategories : Page
    {
        private static string connString => DatabaseHelper.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                LoadCategories();
            }
        }

        private void LoadCategories()
        {
            var categories = new List<CategoryItem>();
            string search = txtSearch.Text.Trim();

            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(@"
                SELECT c.CategoryID, c.Name, c.Description, c.IsActive,
                       (SELECT COUNT(*) FROM dbo.Modules m WHERE m.CategoryID = c.CategoryID) AS ModuleCount
                FROM dbo.Categories c
                WHERE @Search IS NULL OR c.Name LIKE @Search OR c.Description LIKE @Search
                ORDER BY c.Name;", conn))
            {
                cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 110).Value =
                    string.IsNullOrEmpty(search) ? (object)DBNull.Value : "%" + search + "%";
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new CategoryItem
                        {
                            CategoryID = Convert.ToInt32(reader["CategoryID"]),
                            Name = Convert.ToString(reader["Name"]),
                            Description = Convert.ToString(reader["Description"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            ModuleCount = Convert.ToInt32(reader["ModuleCount"])
                        });
                    }
                }
            }

            rptCategories.DataSource = categories;
            rptCategories.DataBind();
            pnlNoCategories.Visible = categories.Count == 0;
        }

        protected void rptCategories_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int categoryId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out categoryId))
                return;

            switch (e.CommandName)
            {
                case "EditCategory":
                    EditCategory(categoryId);
                    break;
                case "ToggleCategory":
                    RunCommand("UPDATE dbo.Categories SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE CategoryID = @ID;",
                        categoryId, "Category status updated.");
                    break;
                case "DeleteCategory":
                    DeleteCategory(categoryId);
                    break;
            }
        }

        private void EditCategory(int categoryId)
        {
            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand("SELECT Name, Description, IsActive FROM dbo.Categories WHERE CategoryID = @ID;", conn))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = categoryId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        ShowError("That category no longer exists.");
                        LoadCategories();
                        return;
                    }

                    ClearForm();
                    lblModalTitle.Text = "Edit Category";
                    hfCategoryID.Value = categoryId.ToString();
                    txtName.Text = Convert.ToString(reader["Name"]);
                    txtDescription.Text = Convert.ToString(reader["Description"]);
                    chkIsActive.Checked = Convert.ToBoolean(reader["IsActive"]);
                }
            }

            UiHelper.ShowModal(this, "modalCategory");
        }

        private void DeleteCategory(int categoryId)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                using (var cmd = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM dbo.Modules WHERE CategoryID = @ID)
                        SELECT -1;
                    ELSE
                    BEGIN
                        DELETE FROM dbo.Categories WHERE CategoryID = @ID;
                        SELECT @@ROWCOUNT;
                    END", conn))
                {
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = categoryId;
                    conn.Open();
                    int result = Convert.ToInt32(cmd.ExecuteScalar());
                    if (result == -1)
                        ShowError("This category still has modules. Move them to another category or deactivate the category instead.");
                    else
                        ShowSuccess("Category deleted.");
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Delete category", ex.Message, ex.StackTrace);
                ShowError("Unable to delete this category. Please try again.");
            }

            LoadCategories();
        }

        private void RunCommand(string sql, int categoryId, string successMessage)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = categoryId;
                    conn.Open();
                    if (cmd.ExecuteNonQuery() == 0)
                        ShowError("That category no longer exists.");
                    else
                        ShowSuccess(successMessage);
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Update category", ex.Message, ex.StackTrace);
                ShowError("Unable to update this category. Please try again.");
            }

            LoadCategories();
        }

        protected void btnSaveCategory_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (name.Length == 0 || name.Length > 100)
            {
                ShowFormError("Enter a category name of up to 100 characters.");
                return;
            }
            if (description.Length > 500)
            {
                ShowFormError("The description must be 500 characters or fewer.");
                return;
            }

            int categoryId;
            bool isNew = !int.TryParse(hfCategoryID.Value, out categoryId) || categoryId <= 0;

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (var check = new SqlCommand(
                        "SELECT COUNT(*) FROM dbo.Categories WHERE Name = @Name AND CategoryID <> @ID;", conn))
                    {
                        check.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
                        check.Parameters.Add("@ID", SqlDbType.Int).Value = isNew ? 0 : categoryId;
                        if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                        {
                            ShowFormError("A category with this name already exists.");
                            return;
                        }
                    }

                    using (var cmd = new SqlCommand(isNew
                        ? "INSERT INTO dbo.Categories (Name, Description, IsActive) VALUES (@Name, @Description, @IsActive);"
                        : "UPDATE dbo.Categories SET Name = @Name, Description = @Description, IsActive = @IsActive WHERE CategoryID = @ID;", conn))
                    {
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                            description.Length == 0 ? (object)DBNull.Value : description;
                        cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = chkIsActive.Checked;
                        if (!isNew)
                            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = categoryId;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Save category", ex.Message, ex.StackTrace);
                ShowFormError("Unable to save this category. Please try again.");
                return;
            }

            ShowSuccess(isNew ? "Category added." : "Category updated.");
            ClearForm();
            LoadCategories();
            UiHelper.HideModal(this, "modalCategory");
        }

        private void ClearForm()
        {
            hfCategoryID.Value = string.Empty;
            txtName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            chkIsActive.Checked = true;
            lblModalTitle.Text = "Add Category";
            pnlModalError.Visible = false;
        }

        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            ClearForm();
            UiHelper.ShowModal(this, "modalCategory");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadCategories();
        }

        private void ShowFormError(string message)
        {
            pnlModalError.Visible = true;
            lblModalError.Text = Server.HtmlEncode(message);
            UiHelper.ShowModal(this, "modalCategory");
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
