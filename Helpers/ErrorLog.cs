using System;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace RespondX.Helpers
{
    /// <summary>
    /// Appends errors to App_Data/RespondX_Errors.log so problems on the hosted site can be diagnosed.
    /// Logging never throws: a failure to log must not hide the original error page.
    /// </summary>
    public static class ErrorLog
    {
        private static readonly object Sync = new object();

        /// <summary>
        /// Logs an unhandled exception for the current request and returns a reference code
        /// that can be shown to the user and matched against the log.
        /// </summary>
        public static string Write(HttpContext context, Exception ex)
        {
            string reference = Guid.NewGuid().ToString("N");
            string path = string.Empty;
            try
            {
                if (context != null && context.Request.Url != null)
                    path = context.Request.Url.AbsolutePath;
            }
            catch
            {
                // Request details are optional.
            }

            Append(string.Format("[{0:u}] Reference {1}; Path {2}{3}{4}{3}{3}",
                DateTime.UtcNow, reference, path, Environment.NewLine, ex));
            return reference;
        }

        public static void Write(string source, string message, string stackTrace)
        {
            Append(string.Format("[{0:u}] {1}: {2}{3}{4}{3}",
                DateTime.UtcNow, source, message, Environment.NewLine, stackTrace ?? string.Empty));
        }

        private static void Append(string entry)
        {
            try
            {
                string logPath = HostingEnvironment.MapPath("~/App_Data/RespondX_Errors.log");
                if (string.IsNullOrEmpty(logPath))
                    return;

                lock (Sync)
                {
                    File.AppendAllText(logPath, entry);
                }
            }
            catch
            {
                // The host may not allow writes to App_Data; ignore.
            }
        }
    }
}
