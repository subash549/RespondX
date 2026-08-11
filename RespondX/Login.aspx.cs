using System;
using System.Web.UI;
using RespondX.Helpers;

namespace RespondX
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // If user is already logged in, redirect to dashboard
            if (SessionHelper.IsUserLoggedIn())
            {
                RedirectToDashboard();
                return;
            }

            // Clear any existing authentication cookies to prevent issues
            if (Request.Cookies[".ASPXAUTH"] != null)
            {
                var cookie = new System.Web.HttpCookie(".ASPXAUTH");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ShowError("Please enter both username and password.");
                    return;
                }

                var user = DatabaseHelper.AuthenticateUser(username, password);

                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        ShowError("Your account has been deactivated. Please contact support.");
                        return;
                    }

                    // Create session
                    SessionHelper.CreateSession(user);
                    DatabaseHelper.UpdateLastLogin(user.UserID);

                    // Redirect based on role
                    RedirectToDashboard();
                }
                else
                {
                    ShowError("Invalid username or password. Please try again.");
                    DatabaseHelper.LogFailedLoginAttempt(username);
                }
            }
            catch (Exception ex)
            {
                ShowError("An error occurred during login. Please try again.");
                DatabaseHelper.LogError("Login Error", ex.Message);
            }
        }

        private void RedirectToDashboard()
        {
            string role = SessionHelper.GetUserRole();

            switch (role)
            {
                case "Admin":
                    Response.Redirect("~/Admin/AdminDashboard.aspx", false);
                    break;
                case "Expert":
                    Response.Redirect("~/Expert/ExpertDashboard.aspx", false);
                    break;
                case "Learner":
                default:
                    Response.Redirect("~/Learner/LearnerDashboard.aspx", false);
                    break;
            }
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
        }
    }
}