using System;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX
{
    public partial class Signup : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessionHelper.IsUserLoggedIn())
            {
                Response.Redirect("~/Login.aspx");
            }
        }

        protected void btnSignup_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    return;
                }

                if (DatabaseHelper.UsernameExists(txtUsername.Text.Trim()))
                {
                    ShowError("Username is already taken. Please choose another.");
                    return;
                }

                if (DatabaseHelper.EmailExists(txtEmail.Text.Trim()))
                {
                    ShowError("Email is already registered. Please use another email or login.");
                    return;
                }

                var user = new User
                {
                    Username = txtUsername.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    PasswordHash = PasswordHelper.HashPassword(txtPassword.Text.Trim(), out string salt),
                    Salt = salt,
                    Role = ddlRole.SelectedValue,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                int userId = DatabaseHelper.CreateUser(user);

                if (userId > 0)
                {
                    if (user.Role == "Learner")
                    {
                        var learner = new Models.Learner
                        {
                            LearnerID = userId,
                            PhoneNumber = "",
                            Address = "",
                            City = "",
                            State = "",
                            ZipCode = "",
                            Organization = "",
                            JobTitle = "",
                            ExperienceYears = 0,
                            Certifications = "",
                            Bio = ""
                        };
                        DatabaseHelper.CreateLearner(learner);
                    }

                    ShowSuccess("Account created successfully! You can now login.");
                    ClearForm();
                    Response.AddHeader("REFRESH", "3;URL=Login.aspx");
                }
                else
                {
                    ShowError("An error occurred while creating your account. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError("An error occurred during registration. Please try again.");
                DatabaseHelper.LogError("Registration Error", ex.Message);
            }
        }

        protected void cvUsername_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = !DatabaseHelper.UsernameExists(args.Value.Trim());
        }

        protected void cvTerms_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = chkTerms.Checked;
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
            pnlSuccess.Visible = false;
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            lblSuccess.Text = message;
            pnlError.Visible = false;
        }

        private void ClearForm()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
            chkTerms.Checked = false;
        }
    }
}