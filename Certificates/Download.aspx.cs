using System;
using System.Web.UI;
using RespondX.Certificates;
using RespondX.Helpers;

namespace RespondX.Certificates
{
    public partial class Download : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            int certificateId;
            if (int.TryParse(Request.QueryString["id"], out certificateId))
            {
                DownloadCertificate(certificateId);
            }
            else
            {
                Response.Redirect("~/Learner/Certificates.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private void DownloadCertificate(int certificateId)
        {
            var learnerId = SessionHelper.GetCurrentUserId();
            if (!learnerId.HasValue)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            var certificate = CertificateHelper.GetEligibleCertificate(certificateId, learnerId.Value, 80m);
            if (certificate == null || !QuizAccessHelper.IsModuleCompleted(certificate.ModuleID, learnerId.Value))
            {
                Response.Redirect("~/Learner/Certificates.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            byte[] certificateImage = CertificateTemplate.GenerateCertificateImage(certificate);
            if (certificateImage == null || certificateImage.Length == 0)
            {
                Response.Redirect("~/Learner/Certificates.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "image/png";
            Response.AddHeader("Content-Disposition", "attachment; filename=RespondX_Certificate_" + certificate.CertificateID + ".png");
            Response.AddHeader("Content-Length", certificateImage.Length.ToString());
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.BinaryWrite(certificateImage);
            Response.Flush();
            Response.End();
        }
    }
}
