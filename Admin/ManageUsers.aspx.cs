using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageUsers : Page
    {
        private const int SqlForeignKeyViolation = 547;
        private static readonly string[] Roles = { "Learner", "Expert", "Admin" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        private int CurrentUserId => SessionHelper.GetCurrentUserId().GetValueOrDefault();

        private void LoadUsers()
        {
            var users = new List<UserItem>();
            var query = @"
                SELECT UserID, Username, Email, FirstName, LastName, Role, IsActive, CreatedAt
                FROM dbo.Users
                WHERE (@Role IS NULL OR Role = @Role)
                  AND (@IsActive IS NULL OR IsActive = @IsActive)
                  AND (@Search IS NULL
                       OR FirstName + N' ' + LastName LIKE @Search
                       OR Email LIKE @Search
                       OR Username LIKE @Search)
                ORDER BY CreatedAt DESC, UserID DESC;";

            string search = txtSearch.Text.Trim();
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 20).Value =
                    ddlRole.SelectedValue == "All" ? (object)DBNull.Value : ddlRole.SelectedValue;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
                    ddlStatus.SelectedValue == "All" ? (object)DBNull.Value : ddlStatus.SelectedValue == "Active";
                cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 120).Value =
                    string.IsNullOrEmpty(search) ? (object)DBNull.Value : "%" + search + "%";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(ReadUser(reader));
                    }
                }
            }

            rptUsers.DataSource = users;
            rptUsers.DataBind();
            pnlNoUsers.Visible = users.Count == 0;
        }

        private static UserItem ReadUser(SqlDataReader reader)
        {
            string firstName = Convert.ToString(reader["FirstName"]);
            string lastName = Convert.ToString(reader["LastName"]);
            return new UserItem
            {
                UserID = Convert.ToInt32(reader["UserID"]),
                Username = Convert.ToString(reader["Username"]),
                Email = Convert.ToString(reader["Email"]),
                FirstName = firstName,
                LastName = lastName,
                FullName = (firstName + " " + lastName).Trim(),
                Role = Convert.ToString(reader["Role"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            };
        }

        private UserItem GetUserById(int userId)
        {
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT UserID, Username, Email, FirstName, LastName, Role, IsActive, CreatedAt
                FROM dbo.Users WHERE UserID = @UserID;", conn))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? ReadUser(reader) : null;
                }
            }
        }

        protected void rptUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int userId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out userId))
                return;

            switch (e.CommandName)
            {
                case "EditUser":
                    EditUser(userId);
                    break;
                case "ToggleUser":
                    ToggleUser(userId);
                    break;
                case "DeleteUser":
                    DeleteUser(userId);
                    break;
            }
        }

        private void EditUser(int userId)
        {
            var user = GetUserById(userId);
            if (user == null)
            {
                ShowError("That user no longer exists.");
                LoadUsers();
                return;
            }

            ClearForm();
            lblModalTitle.Text = "Edit User";
            hfUserID.Value = userId.ToString();
            txtFirstName.Text = user.FirstName;
            txtLastName.Text = user.LastName;
            txtEmail.Text = user.Email;
            txtUsername.Text = user.Username;
            if (ddlUserRole.Items.FindByValue(user.Role) != null)
                ddlUserRole.SelectedValue = user.Role;
            chkIsActive.Checked = user.IsActive;
            lblPasswordRequired.Visible = false;
            lblPasswordHint.Text = "Leave blank to keep the current password.";
            UiHelper.ShowModal(this, "modalUser");
        }

        private void ToggleUser(int userId)
        {
            if (userId == CurrentUserId)
            {
                ShowError("You can't deactivate your own account.");
                return;
            }

            ExecuteUserUpdate("UPDATE dbo.Users SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE UserID = @UserID;",
                userId, "User status updated.", "Unable to update this user's status.");
        }

        private void DeleteUser(int userId)
        {
            if (userId == CurrentUserId)
            {
                ShowError("You can't delete your own account.");
                return;
            }

            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    using (var cmd = new SqlCommand(@"
                        DELETE FROM dbo.Learners WHERE LearnerID = @UserID;
                        DELETE FROM dbo.Users WHERE UserID = @UserID;", conn, transaction))
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
                ShowSuccess("User deleted.");
            }
            catch (SqlException ex) when (ex.Number == SqlForeignKeyViolation)
            {
                ShowError("This user has training history (progress, quiz attempts, certificates or content). Deactivate the account instead.");
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Delete user", ex.Message, ex.StackTrace);
                ShowError("Unable to delete this user. Please try again.");
            }

            LoadUsers();
        }

        private void ExecuteUserUpdate(string sql, int userId, string successMessage, string failureMessage)
        {
            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    conn.Open();
                    if (cmd.ExecuteNonQuery() == 0)
                        ShowError("That user no longer exists.");
                    else
                        ShowSuccess(successMessage);
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Update user", ex.Message, ex.StackTrace);
                ShowError(failureMessage);
            }

            LoadUsers();
        }

        protected void btnSaveUser_Click(object sender, EventArgs e)
        {
            int userId;
            bool isNew = !int.TryParse(hfUserID.Value, out userId) || userId <= 0;

            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string username = txtUsername.Text.Trim();
            string role = ddlUserRole.SelectedValue;
            string password = txtPassword.Text;

            if (firstName.Length == 0 || lastName.Length == 0 || email.Length == 0 || username.Length == 0)
            {
                ShowFormError("First name, last name, email and username are required.");
                return;
            }
            if (firstName.Length > 50 || lastName.Length > 50)
            {
                ShowFormError("Names must be 50 characters or fewer.");
                return;
            }
            if (email.Length > 100 || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowFormError("Enter a valid email address.");
                return;
            }
            if (username.Length < 3 || username.Length > 50 || !Regex.IsMatch(username, @"^[A-Za-z0-9_.-]+$"))
            {
                ShowFormError("Username must be 3-50 characters using letters, numbers, dots, dashes or underscores.");
                return;
            }
            if (Array.IndexOf(Roles, role) < 0)
            {
                ShowFormError("Choose a valid role.");
                return;
            }
            if ((isNew || password.Length > 0) && password.Length < 8)
            {
                ShowFormError(isNew ? "Enter a password of at least 8 characters." : "New password must be at least 8 characters.");
                return;
            }
            if (!isNew && userId == CurrentUserId && (role != "Admin" || !chkIsActive.Checked))
            {
                ShowFormError("You can't remove your own admin role or deactivate your own account.");
                return;
            }

            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    using (var check = new SqlCommand(@"
                        SELECT
                            (SELECT COUNT(*) FROM dbo.Users WHERE Username = @Username AND UserID <> @UserID),
                            (SELECT COUNT(*) FROM dbo.Users WHERE Email = @Email AND UserID <> @UserID);", conn))
                    {
                        check.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                        check.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
                        check.Parameters.Add("@UserID", SqlDbType.Int).Value = isNew ? 0 : userId;
                        using (var reader = check.ExecuteReader())
                        {
                            reader.Read();
                            if (reader.GetInt32(0) > 0)
                            {
                                ShowFormError("That username is already taken.");
                                return;
                            }
                            if (reader.GetInt32(1) > 0)
                            {
                                ShowFormError("That email is already registered.");
                                return;
                            }
                        }
                    }

                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql;
                        if (isNew)
                        {
                            sql = @"
                                INSERT INTO dbo.Users (Username, Email, PasswordHash, Salt, FirstName, LastName, Role, IsActive, CreatedAt)
                                VALUES (@Username, @Email, @PasswordHash, @Salt, @FirstName, @LastName, @Role, @IsActive, GETDATE());
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";
                        }
                        else
                        {
                            sql = @"
                                UPDATE dbo.Users
                                SET Username = @Username, Email = @Email, FirstName = @FirstName, LastName = @LastName,
                                    Role = @Role, IsActive = @IsActive" +
                                    (password.Length > 0 ? ", PasswordHash = @PasswordHash, Salt = @Salt" : "") + @"
                                WHERE UserID = @UserID;
                                SELECT @UserID;";
                        }

                        using (var cmd = new SqlCommand(sql, conn, transaction))
                        {
                            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
                            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = firstName;
                            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = lastName;
                            cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 20).Value = role;
                            cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = chkIsActive.Checked;
                            if (!isNew)
                                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                            if (password.Length > 0)
                            {
                                string salt;
                                cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = PasswordHelper.HashPassword(password, out salt);
                                cmd.Parameters.Add("@Salt", SqlDbType.NVarChar, 50).Value = salt;
                            }

                            userId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Learners need a profile row before they can record progress or take quizzes.
                        if (role == "Learner")
                        {
                            using (var profile = new SqlCommand(@"
                                IF NOT EXISTS (SELECT 1 FROM dbo.Learners WHERE LearnerID = @UserID)
                                    INSERT INTO dbo.Learners (LearnerID, ExperienceYears) VALUES (@UserID, 0);", conn, transaction))
                            {
                                profile.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                                profile.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Save user", ex.Message, ex.StackTrace);
                ShowFormError("Unable to save this user. Please try again.");
                return;
            }

            ShowSuccess(isNew ? "User added." : "User updated.");
            ClearForm();
            LoadUsers();
            UiHelper.HideModal(this, "modalUser");
        }

        private void ClearForm()
        {
            hfUserID.Value = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
            ddlUserRole.SelectedIndex = 0;
            chkIsActive.Checked = true;
            lblModalTitle.Text = "Add User";
            lblPasswordRequired.Visible = true;
            lblPasswordHint.Text = "At least 8 characters.";
            pnlModalError.Visible = false;
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            ClearForm();
            UiHelper.ShowModal(this, "modalUser");
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void ShowFormError(string message)
        {
            pnlModalError.Visible = true;
            lblModalError.Text = Server.HtmlEncode(message);
            UiHelper.ShowModal(this, "modalUser");
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
