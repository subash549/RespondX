using System;
using System.Web.UI;
using RespondX.Helpers;

namespace RespondX
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Don't redirect on Login, Signup, or Logout pages
            string pageName = System.IO.Path.GetFileName(Request.PhysicalPath);
            if (pageName.Equals("Login.aspx", StringComparison.OrdinalIgnoreCase) ||
                pageName.Equals("Signup.aspx", StringComparison.OrdinalIgnoreCase) ||
                pageName.Equals("Logout.aspx", StringComparison.OrdinalIgnoreCase))
            {
                pnlNavbar.Visible = false;
                return;
            }

            // Check if user is logged in
            if (SessionHelper.IsUserLoggedIn())
            {
                pnlNavbar.Visible = true;
                pnlNavLinks.Visible = true;
            }
            else
            {
                pnlNavbar.Visible = false;
                // Redirect to login if not authenticated and not on login page
                Response.Redirect("~/Login.aspx");
            }
        }
    }
}