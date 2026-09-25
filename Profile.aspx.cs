using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX
{
    public partial class Profile : Page
    {
        private const int MaxImageBytes = 5 * 1024 * 1024;
        private string CurrentImagePath
        {
            get { return Convert.ToString(ViewState["ProfileImagePath"]); }
            set { ViewState["ProfileImagePath"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireAuthentication()) return;
            if (!IsPostBack) LoadProfile();
        }

        private void LoadProfile()
        {
            int? id = SessionHelper.GetCurrentUserId();
            if (!id.HasValue) return;
            User user = DatabaseHelper.GetUserById(id.Value);
            if (user == null) return;
            txtFirstName.Text = user.FirstName;
            txtLastName.Text = user.LastName;
            txtEmail.Text = user.Email;
            lblFullName.Text = user.FullName;
            lblRole.Text = user.Role;
            lblMemberSince.Text = user.CreatedAt.ToString("MMMM yyyy");
            CurrentImagePath = user.ProfileImage;
            SetProfileImage(user.ProfileImage);

            bool isLearner = string.Equals(user.Role, "Learner", StringComparison.OrdinalIgnoreCase);
            pnlLearnerFields.Visible = isLearner;
            pnlProfessional.Visible = isLearner;
            pnlLearnerStats.Visible = isLearner;
            if (isLearner)
            {
                DatabaseHelper.EnsureLearnerProfile(user.UserID);
                Models.Learner learner = DatabaseHelper.GetLearnerById(user.UserID);
                if (learner != null)
                {
                    txtPhoneNumber.Text = learner.PhoneNumber;
                    txtDateOfBirth.Text = learner.DateOfBirth.HasValue ? learner.DateOfBirth.Value.ToString("yyyy-MM-dd") : string.Empty;
                    txtExperience.Text = learner.ExperienceYears.ToString();
                    txtOrganization.Text = learner.Organization;
                    txtJobTitle.Text = learner.JobTitle;
                    txtBio.Text = learner.Bio;
                }
                LoadLearnerStats(user.UserID);
            }
        }

        private void LoadLearnerStats(int learnerId)
        {
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT
                    (SELECT COUNT(DISTINCT ModuleID) FROM dbo.LearnerProgress WHERE LearnerID = @LearnerID AND Status = N'Completed') AS Modules,
                    (SELECT COUNT(*) FROM dbo.QuizAttempts WHERE LearnerID = @LearnerID) AS Quizzes,
                    (SELECT COUNT(*) FROM dbo.Certificates WHERE LearnerID = @LearnerID) AS Certificates;", conn))
            {
                cmd.Parameters.Add("@LearnerID", System.Data.SqlDbType.Int).Value = learnerId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblModuleCount.Text = Convert.ToString(reader["Modules"]);
                        lblQuizCount.Text = Convert.ToString(reader["Quizzes"]);
                        lblCertificateCount.Text = Convert.ToString(reader["Certificates"]);
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            if (firstName.Length == 0 || lastName.Length == 0 || firstName.Length > 50 || lastName.Length > 50)
            { ShowError("Enter first and last names (up to 50 characters each)."); return; }
            if (email.Length > 100 || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            { ShowError("Enter a valid email address."); return; }

            string savedPath = CurrentImagePath;
            string newPhysicalPath = null;
            try
            {
                int? id = SessionHelper.GetCurrentUserId();
                if (!id.HasValue) return;
                if (fuProfileImage.HasFile)
                {
                    string extension = Path.GetExtension(fuProfileImage.FileName).ToLowerInvariant();
                    if (fuProfileImage.PostedFile.ContentLength <= 0 || fuProfileImage.PostedFile.ContentLength > MaxImageBytes)
                    { ShowError("Choose an image smaller than 5 MB."); return; }
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                    { ShowError("Use a JPG, PNG or GIF image."); return; }
                    using (var imageStream = fuProfileImage.FileContent)
                    using (var image = Image.FromStream(imageStream, false, true))
                    {
                        if (image.Width > 6000 || image.Height > 6000)
                        { ShowError("The image dimensions are too large."); return; }
                    }

                    string folder = Server.MapPath("~/Uploads/Profiles");
                    Directory.CreateDirectory(folder);
                    string fileName = Guid.NewGuid().ToString("N") + extension;
                    newPhysicalPath = Path.Combine(folder, fileName);
                    fuProfileImage.SaveAs(newPhysicalPath);
                    savedPath = "~/Uploads/Profiles/" + fileName;
                }

                DatabaseHelper.UpdateProfile(id.Value, firstName, lastName, email, savedPath);
                if (string.Equals(SessionHelper.GetCurrentUser().Role, "Learner", StringComparison.OrdinalIgnoreCase))
                    UpdateLearnerProfile(id.Value);
                DeletePreviousProfileImage(CurrentImagePath, savedPath);
                User current = SessionHelper.GetCurrentUser();
                current.FirstName = firstName;
                current.LastName = lastName;
                current.Email = email;
                current.ProfileImage = savedPath;
                SessionHelper.CreateSession(current);
                CurrentImagePath = savedPath;
                SetProfileImage(savedPath);
                lblFullName.Text = current.FullName;
                ShowSuccess("Profile updated successfully.");
            }
            catch (FormatException)
            {
                ShowError("Enter a valid date of birth and a whole number of years of experience.");
            }
            catch (ArgumentOutOfRangeException)
            {
                ShowError("Enter valid profile details.");
            }
            catch (System.Data.SqlClient.SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                if (newPhysicalPath != null && File.Exists(newPhysicalPath)) File.Delete(newPhysicalPath);
                ShowError("That email address is already in use.");
            }
            catch (Exception ex)
            {
                if (newPhysicalPath != null && File.Exists(newPhysicalPath)) File.Delete(newPhysicalPath);
                DatabaseHelper.LogError("Save profile", ex.Message, ex.StackTrace);
                ShowError("Unable to save your profile. Please try again.");
            }
        }

        private void UpdateLearnerProfile(int learnerId)
        {
            DateTime? dateOfBirth = null;
            if (!string.IsNullOrWhiteSpace(txtDateOfBirth.Text))
            {
                DateTime parsed;
                if (!DateTime.TryParse(txtDateOfBirth.Text, out parsed) || parsed.Date >= DateTime.Today)
                    throw new FormatException();
                dateOfBirth = parsed.Date;
            }

            int experience;
            if (!int.TryParse(txtExperience.Text, out experience) || experience < 0 || experience > 100)
                throw new FormatException();

            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                UPDATE dbo.Learners SET DateOfBirth=@DateOfBirth, PhoneNumber=@PhoneNumber, Organization=@Organization,
                    JobTitle=@JobTitle, ExperienceYears=@ExperienceYears, Bio=@Bio WHERE LearnerID=@LearnerID;", conn))
            {
                cmd.Parameters.Add("@LearnerID", System.Data.SqlDbType.Int).Value = learnerId;
                cmd.Parameters.Add("@DateOfBirth", System.Data.SqlDbType.Date).Value = (object)dateOfBirth ?? DBNull.Value;
                cmd.Parameters.Add("@PhoneNumber", System.Data.SqlDbType.NVarChar, 20).Value = DbValue(txtPhoneNumber.Text, 20);
                cmd.Parameters.Add("@Organization", System.Data.SqlDbType.NVarChar, 100).Value = DbValue(txtOrganization.Text, 100);
                cmd.Parameters.Add("@JobTitle", System.Data.SqlDbType.NVarChar, 100).Value = DbValue(txtJobTitle.Text, 100);
                cmd.Parameters.Add("@ExperienceYears", System.Data.SqlDbType.Int).Value = experience;
                cmd.Parameters.Add("@Bio", System.Data.SqlDbType.NVarChar, 500).Value = DbValue(txtBio.Text, 500);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static object DbValue(string value, int maxLength)
        {
            string trimmed = (value ?? string.Empty).Trim();
            if (trimmed.Length > maxLength) throw new ArgumentOutOfRangeException();
            return trimmed.Length == 0 ? (object)DBNull.Value : trimmed;
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            string current = txtCurrentPassword.Text;
            string password = txtNewPassword.Text;
            if (string.IsNullOrEmpty(current) || password.Length < 8 || password != txtConfirmPassword.Text)
            { ShowError("Enter your current password, a new password of at least 8 characters, and matching confirmation."); return; }
            try
            {
                int? id = SessionHelper.GetCurrentUserId();
                if (id.HasValue) DatabaseHelper.ChangePassword(id.Value, current, password);
                txtCurrentPassword.Text = txtNewPassword.Text = txtConfirmPassword.Text = string.Empty;
                ShowSuccess("Password changed successfully.");
            }
            catch (InvalidOperationException ex) { ShowError(ex.Message); }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Change profile password", ex.Message, ex.StackTrace);
                ShowError("Unable to change your password. Please try again.");
            }
        }

        private void SetProfileImage(string path)
        {
            imgProfile.ImageUrl = string.IsNullOrWhiteSpace(path) ? ResolveUrl("~/Content/Images/logo.svg") : ResolveUrl(path);
        }

        private void DeletePreviousProfileImage(string previousPath, string currentPath)
        {
            if (string.IsNullOrWhiteSpace(previousPath) || string.Equals(previousPath, currentPath, StringComparison.OrdinalIgnoreCase)) return;
            string prefix = "~/Uploads/Profiles/";
            if (!previousPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return;
            string oldFile = Server.MapPath(previousPath);
            if (File.Exists(oldFile)) File.Delete(oldFile);
        }

        private void ShowSuccess(string message) { pnlSuccess.Visible = true; lblSuccess.Text = Server.HtmlEncode(message); pnlError.Visible = false; }
        private void ShowError(string message) { pnlError.Visible = true; lblError.Text = Server.HtmlEncode(message); pnlSuccess.Visible = false; }
    }
}
