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

            // Register routes and bundles
            RouteConfig.RegisterRoutes(System.Web.Routing.RouteTable.Routes);
            BundleConfig.RegisterBundles(System.Web.Optimization.BundleTable.Bundles);

            ModuleMediaHelper.EnsureModuleMediaColumns();
            try
            {
                NotificationRepository.EnsureSchema();
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Ensure notification schema", ex.Message, ex.StackTrace);
            }
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

            // If the error page itself fails, don't redirect back to it (friendly URLs serve it as /Error).
            string requestPath = Request.Path.TrimEnd('/');
            if (requestPath.EndsWith("/Error", StringComparison.OrdinalIgnoreCase) ||
                requestPath.EndsWith("/Error.aspx", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (ex == null)
            {
                return;
            }

            var httpException = ex as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404)
            {
                Server.ClearError();
                Response.Redirect("~/Error.aspx?code=404", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            string errorReference = ErrorLog.Write(Context, ex);
            Server.ClearError();
            Response.Redirect("~/Error.aspx?ref=" + HttpUtility.UrlEncode(errorReference), false);
            Context.ApplicationInstance.CompleteRequest();
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
