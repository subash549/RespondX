using System;
using System.IO;
using System.Web.UI;
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
                Response.Redirect("~/Learner/Certificates.aspx");
            }
        }

        private void DownloadCertificate(int certificateId)
        {
            // In production, get certificate from database
            // For now, redirect back
            Response.Redirect("~/Learner/Certificates.aspx");
        }
    }
}