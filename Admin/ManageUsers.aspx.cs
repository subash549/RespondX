using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageUsers : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            var users = GetUsers();

            var role = ddlRole.SelectedValue;
            if (role != "All")
            {
                users = users.FindAll(u => u.Role == role);
            }

            var status = ddlStatus.SelectedValue;
            if (status != "All")
            {
                bool isActive = status == "Active";
                users = users.FindAll(u => u.IsActive == isActive);
            }

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                users = users.FindAll(u =>
                    u.FullName.ToLower().Contains(search.ToLower()) ||
                    u.Email.ToLower().Contains(search.ToLower()) ||
                    u.Username.ToLower().Contains(search.ToLower())
                );
            }

            rptUsers.DataSource = users;
            rptUsers.DataBind();
        }

        private List<UserItem> GetUsers()
        {
            return new List<UserItem>
            {
                new UserItem { UserID = 1, FullName = "John Smith", Email = "john@email.com", Username = "johnsmith", Role = "Learner", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-2) },
                new UserItem { UserID = 2, FullName = "Mary Johnson", Email = "mary@email.com", Username = "maryj", Role = "Expert", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-1) },
                new UserItem { UserID = 3, FullName = "Robert Wilson", Email = "robert@email.com", Username = "robertw", Role = "Learner", IsActive = false, CreatedAt = DateTime.Now.AddDays(-5) },
                new UserItem { UserID = 4, FullName = "Jane Doe", Email = "jane@email.com", Username = "janedoe", Role = "Admin", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-3) }
            };
        }

        protected void rptUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int userId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Edit":
                    EditUser(userId);
                    break;
                case "Toggle":
                    ToggleUser(userId);
                    break;
                case "Delete":
                    DeleteUser(userId);
                    break;
            }
        }

        private void EditUser(int userId)
        {
            var user = GetUserById(userId);
            if (user != null)
            {
                lblModalTitle.Text = "Edit User";
                hfUserID.Value = userId.ToString();
                txtFirstName.Text = user.FirstName;
                txtLastName.Text = user.LastName;
                txtEmail.Text = user.Email;
                txtUsername.Text = user.Username;
                ddlUserRole.SelectedValue = user.Role;
                txtPassword.Text = "";
                txtPassword.Visible = true;
                txtPassword.Attributes["placeholder"] = "Leave blank to keep current password";

                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalUser').modal('show');", true);
            }
        }

        private void ToggleUser(int userId)
        {
            ShowSuccess("User status updated successfully");
            LoadUsers();
        }

        private void DeleteUser(int userId)
        {
            ShowSuccess("User deleted successfully");
            LoadUsers();
        }

        protected void btnSaveUser_Click(object sender, EventArgs e)
        {
            int userId;
            bool isNew = !int.TryParse(hfUserID.Value, out userId) || userId == 0;

            if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text) ||
                string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtUsername.Text))
            {
                ShowError("All required fields must be filled.");
                return;
            }

            ShowSuccess(isNew ? "User added successfully" : "User updated successfully");
            ClearForm();
            LoadUsers();

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalUser').modal('hide');", true);
        }

        private UserItem GetUserById(int userId)
        {
            var users = GetUsers();
            return users.Find(u => u.UserID == userId);
        }

        private void ClearForm()
        {
            hfUserID.Value = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
            ddlUserRole.SelectedIndex = 0;
            lblModalTitle.Text = "Add User";
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            ClearForm();
            txtPassword.Visible = true;
            txtPassword.Attributes["placeholder"] = "Enter password for new user";
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalUser').modal('show');", true);
        }

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUsers();
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUsers();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUsers();
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