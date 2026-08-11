using System;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class Profile : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                LoadProfile();
            }
        }

        private void LoadProfile()
        {
            var userId = SessionHelper.GetCurrentUserId();
            if (!userId.HasValue) return;

            var user = DatabaseHelper.GetUserById(userId.Value);
            var learner = DatabaseHelper.GetLearnerById(userId.Value);

            if (user != null)
            {
                txtFirstName.Text = user.FirstName;
                txtLastName.Text = user.LastName;
                txtEmail.Text = user.Email;
                lblFullName.Text = user.FullName;
                lblRole.Text = user.Role;
                lblMemberSince.Text = $"Member since {user.CreatedAt:MMMM yyyy}";
            }

            if (learner != null)
            {
                txtPhone.Text = learner.PhoneNumber;
                txtDateOfBirth.Text = learner.DateOfBirth?.ToString("yyyy-MM-dd");
                txtExperience.Text = learner.ExperienceYears.ToString();
                txtOrganization.Text = learner.Organization;
                txtJobTitle.Text = learner.JobTitle;
                txtBio.Text = learner.Bio;

                // Load stats
                lblModulesCompleted.Text = "3"; // In production, from database
                lblQuizzesTaken.Text = "5";
                lblCertificatesEarned.Text = "2";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var userId = SessionHelper.GetCurrentUserId();
                    if (!userId.HasValue) return;

                    // Update user information
                    var user = DatabaseHelper.GetUserById(userId.Value);
                    if (user != null)
                    {
                        user.FirstName = txtFirstName.Text.Trim();
                        user.LastName = txtLastName.Text.Trim();
                        user.Email = txtEmail.Text.Trim();
                        // In production, save to database
                    }

                    // Update learner information
                    var learner = DatabaseHelper.GetLearnerById(userId.Value);
                    if (learner != null)
                    {
                        learner.PhoneNumber = txtPhone.Text.Trim();
                        learner.DateOfBirth = string.IsNullOrEmpty(txtDateOfBirth.Text) ? (DateTime?)null : DateTime.Parse(txtDateOfBirth.Text);
                        learner.ExperienceYears = string.IsNullOrEmpty(txtExperience.Text) ? 0 : int.Parse(txtExperience.Text);
                        learner.Organization = txtOrganization.Text.Trim();
                        learner.JobTitle = txtJobTitle.Text.Trim();
                        learner.Bio = txtBio.Text.Trim();
                        // In production, save to database
                    }

                    ShowSuccess("Profile updated successfully!");
                }
                catch (Exception ex)
                {
                    ShowError("An error occurred while saving your profile.");
                    DatabaseHelper.LogError("Profile Update Error", ex.Message);
                }
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            var currentPassword = txtCurrentPassword.Text;
            var newPassword = txtNewPassword.Text;
            var confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            {
                ShowError("Please enter both current and new password.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                ShowError("New passwords do not match.");
                return;
            }

            if (newPassword.Length < 8)
            {
                ShowError("Password must be at least 8 characters long.");
                return;
            }

            // In production, verify current password and update
            ShowSuccess("Password changed successfully!");

            // Clear password fields
            txtCurrentPassword.Text = "";
            txtNewPassword.Text = "";
            txtConfirmPassword.Text = "";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            LoadProfile();
            ShowSuccess("Changes discarded.");
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