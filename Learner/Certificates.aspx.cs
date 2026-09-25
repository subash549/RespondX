using System;
using System.Collections.Generic;
using System.Linq;
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

            var certificates = CertificateHelper.GetCertificatesByLearner(userId.Value)
                .Where(certificate => certificate.Score > 80m)
                .Select(certificate => new CertificateItem
                {
                    CertificateID = certificate.CertificateID,
                    ModuleTitle = certificate.ModuleTitle,
                    LearnerName = certificate.LearnerName,
                    IssueDate = certificate.IssueDate,
                    Score = certificate.Score,
                    VerificationCode = certificate.VerificationCode,
                    IsValid = certificate.IsValid,
                    ExpiryDate = certificate.ExpiryDate,
                    DownloadUrl = ResolveUrl("~/Certificates/Download.aspx?id=" + certificate.CertificateID),
                    ViewUrl = string.Empty
                })
                .ToList();

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
