using System;
using System.Web;
using System.Web.UI;

namespace RespondX.Helpers
{
    /// <summary>
    /// Server-side shortcuts for the client behaviour in Scripts/RespondX.js.
    /// Uses ScriptManager so the scripts also run after UpdatePanel (partial) postbacks.
    /// </summary>
    public static class UiHelper
    {
        /// <summary>Shows a toast. Type is success, error, warning or info.</summary>
        public static void Notify(Page page, string message, string type = "success")
        {
            string script = "showNotification(" +
                HttpUtility.JavaScriptStringEncode(message ?? string.Empty, true) + ", " +
                HttpUtility.JavaScriptStringEncode(type ?? "info", true) + ");";
            ScriptManager.RegisterStartupScript(page, typeof(UiHelper), "notify-" + Guid.NewGuid().ToString("N"), script, true);
        }

        public static void ShowModal(Page page, string modalId)
        {
            ScriptManager.RegisterStartupScript(page, typeof(UiHelper), "modal-" + modalId,
                "$('#" + modalId + "').modal('show');", true);
        }

        public static void HideModal(Page page, string modalId)
        {
            ScriptManager.RegisterStartupScript(page, typeof(UiHelper), "modal-" + modalId,
                "$('#" + modalId + "').modal('hide');", true);
        }

        /// <summary>Label for a 1-5 scenario difficulty level.</summary>
        public static string DifficultyLabel(int level)
        {
            switch (level)
            {
                case 1: return "Beginner";
                case 2: return "Easy";
                case 3: return "Intermediate";
                case 4: return "Advanced";
                case 5: return "Expert";
                default: return "Intermediate";
            }
        }

        /// <summary>Formats a date as "5 minutes ago", "3 days ago", etc.</summary>
        public static string TimeAgo(DateTime value)
        {
            var span = DateTime.Now - value;
            if (span.TotalMinutes < 1) return "just now";
            if (span.TotalMinutes < 60) return Plural((int)span.TotalMinutes, "minute");
            if (span.TotalHours < 24) return Plural((int)span.TotalHours, "hour");
            if (span.TotalDays < 30) return Plural((int)span.TotalDays, "day");
            return value.ToString("MMM dd, yyyy");
        }

        private static string Plural(int count, string unit)
        {
            return count + " " + unit + (count == 1 ? "" : "s") + " ago";
        }
    }
}
