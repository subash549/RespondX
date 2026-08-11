using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class Certificates : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                LoadCertificates();
            }
        }

        private void LoadCertificates()
        {
            var userId = SessionHelper.GetCurrentUserId();
            if (!userId.HasValue) return;

            var certificates = new List<CertificateItem>
            {
                new CertificateItem
                {
                    CertificateID = 1,
                    ModuleTitle = "Introduction to Emergency Response",
                    LearnerName = "John Doe",
                    IssueDate = DateTime.Now.AddMonths(-1),
                    Score = 85,
                    VerificationCode = "RX-2024-12345",
                    IsValid = true,
                    ExpiryDate = DateTime.Now.AddYears(2),
                    DownloadUrl = "#",
                    ViewUrl = "#"
                },
                new CertificateItem
                {
                    CertificateID = 2,
                    ModuleTitle = "CPR and First Aid",
                    LearnerName = "John Doe",
                    IssueDate = DateTime.Now.AddDays(-5),
                    Score = 92,
                    VerificationCode = "RX-2024-12346",
                    IsValid = true,
                    ExpiryDate = DateTime.Now.AddYears(2),
                    DownloadUrl = "#",
                    ViewUrl = "#"
                }
            };

            rptCertificates.DataSource = certificates;
            rptCertificates.DataBind();
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                string code = btn.CommandArgument;
                bool isValid = CertificateHelper.VerifyCertificate(code);

                string message = isValid ?
                    "Certificate is valid and authentic." :
                    "Certificate not found or has been revoked.";

                string type = isValid ? "success" : "error";

                ClientScript.RegisterStartupScript(this.GetType(), "verify",
                    $"showNotification('{message}', '{type}');", true);
            }
        }
    }
}