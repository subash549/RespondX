using System;
using System.Web.UI;
using RespondX.Helpers;

namespace RespondX
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool isLoggedIn = SessionHelper.IsUserLoggedIn();
            phPublicNavigation.Visible = !isLoggedIn;

            if (isLoggedIn)
            {
                phGuestLinks.Visible = false;
                
                string role = SessionHelper.GetUserRole();
                if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    phAdminLinks.Visible = true;
                    phLearnerLinks.Visible = false;
                    phExpertLinks.Visible = false;
                }
                else if (string.Equals(role, "Expert", StringComparison.OrdinalIgnoreCase))
                {
                    phExpertLinks.Visible = true;
                    phAdminLinks.Visible = false;
                    phLearnerLinks.Visible = false;
                }
                else
                {
                    // Default to Learner
                    phLearnerLinks.Visible = true;
                    phAdminLinks.Visible = false;
                    phExpertLinks.Visible = false;
                }
            }
            else
            {
                phPublicNavigation.Visible = true;
                phGuestLinks.Visible = true;
                phLearnerLinks.Visible = false;
                phAdminLinks.Visible = false;
                phExpertLinks.Visible = false;
            }
        }
    }
}
