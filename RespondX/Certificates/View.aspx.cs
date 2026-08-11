using System;
using System.Web.UI;
using RespondX.Helpers;

namespace RespondX.Certificates
{
    public partial class View : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                int certificateId;
                if (int.TryParse(Request.QueryString["id"], out certificateId))
                {
                    LoadCertificate(certificateId);
                }
                else
                {
                    pnlCertificate.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadCertificate(int certificateId)
        {
            // In production, load certificate from database
            // For now, show placeholder
            pnlCertificate.Visible = true;
            lblTitle.Text = "Certificate of Completion";
            lblSubtitle.Text = "RespondX Emergency Response Training";
        }
    }
}