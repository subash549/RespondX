using System;
using RespondX.Helpers;

namespace RespondX
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Stop IIS from replacing this page with its own error screen.
            Response.TrySkipIisCustomErrors = true;

            if (Request.QueryString["code"] == "404")
            {
                Response.StatusCode = 404;
                Page.Title = "Page not found - RespondX";
                litHeading.Text = "Page not found";
                litMessage.Text = "The page you were looking for doesn't exist or may have been moved.";
                imgIllustration.ImageUrl = "~/Content/Images/not-found.svg";
            }
            else
            {
                Response.StatusCode = 500;
                Guid errorReference;
                if (Guid.TryParseExact(Request.QueryString["ref"], "N", out errorReference))
                {
                    pnlErrorReference.Visible = true;
                    lblErrorReference.Text = errorReference.ToString("N");
                }
            }

            if (SessionHelper.IsUserLoggedIn())
            {
                lnkHome.NavigateUrl = AuthorizationHelper.GetDashboardUrl();
                lnkHome.Text = "Go to Dashboard";
            }
        }
    }
}
