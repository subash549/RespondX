using System;
using RespondX.Helpers;

namespace RespondX
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clear session
            SessionHelper.ClearSession();

            // Abandon session
            Session.Abandon();

            // Clear cookies
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            }

            if (Request.Cookies[".ASPXAUTH"] != null)
            {
                Response.Cookies[".ASPXAUTH"].Expires = DateTime.Now.AddDays(-1);
            }

            // Redirect to login page after 2 seconds
            Response.AddHeader("REFRESH", "2;URL=Login.aspx");
        }
    }
}