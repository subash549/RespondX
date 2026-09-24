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
    public partial class ManageCategories : Page
    {
        private string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString ?? "Data Source=DESKTOP-5UH7Q5H\\SQLEXPRESS01;Initial Catalog=RespondX;Integrated Security=True;TrustServerCertificate=True;";

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
            using (var conn = new SqlConnection(connString))
            {
                var query = @"
                    SELECT c.CategoryID, c.Name, c.Description, c.IsActive, 
                           (SELECT COUNT(*) FROM Modules m WHERE m.CategoryID = c.CategoryID) AS ModuleCount 
                    FROM Categories c";
                var search = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(search))
                {
                    query += " WHERE c.Name LIKE @Search OR c.Description LIKE @Search";
                }
                
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        cmd.Parameters.AddWithValue("@Search", "%" + search + "%");
                    }
                    
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new CategoryItem
                            {
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                ModuleCount = Convert.ToInt32(reader["ModuleCount"])
                            });
                        }
                    }
                }
            }

            rptCategories.DataSource = categories;
            rptCategories.DataBind();
        }

        protected void rptCategories_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int categoryId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Edit")
            {
                EditCategory(categoryId);
            }
            else if (e.CommandName == "Toggle")
            {
                ToggleCategory(categoryId);
            }
        }

        private void EditCategory(int categoryId)
        {
            using (var conn = new SqlConnection(connString))
            {
                using (var cmd = new SqlCommand("SELECT Name, Description, IsActive FROM Categories WHERE CategoryID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", categoryId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblModalTitle.Text = "Edit Category";
                            hfCategoryID.Value = categoryId.ToString();
                            txtName.Text = reader["Name"].ToString();
                            txtDescription.Text = reader["Description"].ToString();
                            chkIsActive.Checked = Convert.ToBoolean(reader["IsActive"]);

                            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalCategory').modal('show');", true);
                        }
                    }
                }
            }
        }

        private void ToggleCategory(int categoryId)
        {
            using (var conn = new SqlConnection(connString))
            {
                using (var cmd = new SqlCommand("UPDATE Categories SET IsActive = ~IsActive WHERE CategoryID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", categoryId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            ShowSuccess("Category status updated successfully");
            LoadCategories();
        }

        protected void btnSaveCategory_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                ShowError("Name is required.");
                return;
            }

            int categoryId;
            bool isNew = !int.TryParse(hfCategoryID.Value, out categoryId) || categoryId == 0;

            using (var conn = new SqlConnection(connString))
            {
                string query = isNew ? 
                    "INSERT INTO Categories (Name, Description, IsActive) VALUES (@Name, @Description, @IsActive)" : 
                    "UPDATE Categories SET Name = @Name, Description = @Description, IsActive = @IsActive WHERE CategoryID = @ID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);
                    if (!isNew)
                    {
                        cmd.Parameters.AddWithValue("@ID", categoryId);
                    }
                    
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ShowSuccess(isNew ? "Category added successfully" : "Category updated successfully");
            ClearForm();
            LoadCategories();

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalCategory').modal('hide');", true);
        }

        private void ClearForm()
        {
            hfCategoryID.Value = "";
            txtName.Text = "";
            txtDescription.Text = "";
            chkIsActive.Checked = true;
            lblModalTitle.Text = "Add Category";
        }

        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalCategory').modal('show');", true);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadCategories();
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
