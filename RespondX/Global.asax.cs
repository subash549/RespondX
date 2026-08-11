using System;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using RespondX.Helpers;

namespace RespondX
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Application["ApplicationName"] = "RespondX";
            Application["Version"] = "1.0.0";
            Application["StartTime"] = DateTime.Now;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["SessionID"] = Session.SessionID;
            Session["StartTime"] = DateTime.Now;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Don't redirect for Login, Signup, or static resources
            string path = Request.Path.ToLower();
            if (path.Contains("/login.aspx") ||
                path.Contains("/signup.aspx") ||
                path.Contains("/logout.aspx") ||
                path.Contains("/content/") ||
                path.Contains("/scripts/") ||
                path.Contains("/fonts/") ||
                path.Contains(".css") ||
                path.Contains(".js") ||
                path.Contains(".png") ||
                path.Contains(".jpg") ||
                path.Contains(".gif"))
            {
                return;
            }

            // Check if user is authenticated
            bool isAuthenticated = Request.IsAuthenticated || SessionHelper.IsUserLoggedIn();

            if (!isAuthenticated)
            {
                // Not authenticated, let FormsAuthentication handle it
                return;
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // Handle authentication if needed
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                DatabaseHelper.LogError("Global Error", ex.Message, ex.StackTrace);
                Server.ClearError();
                Response.Redirect("~/Error.aspx");
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Session ended
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Application ended
        }
    }
}