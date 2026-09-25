using System;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using RespondX.Helpers;

namespace RespondX
{
    public class Global : System.Web.HttpApplication
    {
        private static readonly object ErrorLogLock = new object();

        protected void Application_Start(object sender, EventArgs e)
        {
            Application["ApplicationName"] = "RespondX";
            Application["Version"] = "1.0.0";
            Application["StartTime"] = DateTime.Now;

            // Register routes and bundles
            RouteConfig.RegisterRoutes(System.Web.Routing.RouteTable.Routes);
            BundleConfig.RegisterBundles(System.Web.Optimization.BundleTable.Bundles);

            ModuleMediaHelper.EnsureModuleMediaColumns();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["SessionID"] = Session.SessionID;
            Session["StartTime"] = DateTime.Now;
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // Handle authentication if needed
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex is HttpUnhandledException && ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            if (ex is System.Threading.ThreadAbortException)
            {
                return; // Ignore ThreadAbortException
            }

            // Also check if the request is already for Error.aspx to prevent infinite loops
            if (Request.Path.IndexOf("Error.aspx", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return;
            }

            if (ex != null)
            {
                DatabaseHelper.LogError("Global Error", ex.Message, ex.StackTrace);
                string errorReference = Guid.NewGuid().ToString("N");
                try
                {
                    string logPath = Server.MapPath("~/App_Data/RespondX_Errors.log");
                    string logEntry = string.Format(
                        "[{0:u}] Reference {1}; Path {2}{3}{4}{3}{3}",
                        DateTime.UtcNow,
                        errorReference,
                        Request.Url == null ? string.Empty : Request.Url.AbsolutePath,
                        Environment.NewLine,
                        ex);
                    lock (ErrorLogLock)
                    {
                        File.AppendAllText(logPath, logEntry);
                    }
                }
                catch
                {
                    // Logging must not prevent the error page from being shown.
                }

                Server.ClearError();
                Response.Redirect("~/Error.aspx?ref=" + HttpUtility.UrlEncode(errorReference), false);
                Context.ApplicationInstance.CompleteRequest();
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
