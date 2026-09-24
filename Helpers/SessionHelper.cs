using System;
using System.Web;
using RespondX.Models;

namespace RespondX.Helpers
{
    public static class SessionHelper
    {
        private const string UserSessionKey = "CurrentUser";
        private const string UserIdKey = "UserId";
        private const string UserRoleKey = "UserRole";
        private const string UserNameKey = "UserName";
        private const string SessionStartKey = "SessionStart";
        private const string LastActivityKey = "LastActivity";

        /// <summary>
        /// Creates a user session
        /// </summary>
        public static void CreateSession(User user)
        {
            HttpContext.Current.Session[UserSessionKey] = user;
            HttpContext.Current.Session[UserIdKey] = user.UserID;
            HttpContext.Current.Session[UserRoleKey] = user.Role;
            HttpContext.Current.Session[UserNameKey] = user.FirstName + " " + user.LastName;
            HttpContext.Current.Session[SessionStartKey] = DateTime.Now;
            HttpContext.Current.Session[LastActivityKey] = DateTime.Now;
        }

        /// <summary>
        /// Gets the current user from session
        /// </summary>
        public static User GetCurrentUser()
        {
            return HttpContext.Current.Session[UserSessionKey] as User;
        }

        /// <summary>
        /// Gets the current user ID
        /// </summary>
        public static int? GetCurrentUserId()
        {
            var userId = HttpContext.Current.Session[UserIdKey];
            return userId != null ? Convert.ToInt32(userId) : (int?)null;
        }

        /// <summary>
        /// Gets the current user's role
        /// </summary>
        public static string GetUserRole()
        {
            return HttpContext.Current.Session[UserRoleKey] as string;
        }

        /// <summary>
        /// Gets the current user's display name
        /// </summary>
        public static string GetUserName()
        {
            return HttpContext.Current.Session[UserNameKey] as string;
        }

        /// <summary>
        /// Checks if a user is logged in
        /// </summary>
        public static bool IsUserLoggedIn()
        {
            return HttpContext.Current.Session[UserSessionKey] != null;
        }

        /// <summary>
        /// Clears the session
        /// </summary>
        public static void ClearSession()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }

        /// <summary>
        /// Updates the last activity time
        /// </summary>
        public static void UpdateLastActivity()
        {
            HttpContext.Current.Session[LastActivityKey] = DateTime.Now;
        }

        /// <summary>
        /// Gets the session start time
        /// </summary>
        public static DateTime? GetSessionStartTime()
        {
            var start = HttpContext.Current.Session[SessionStartKey];
            return start != null ? (DateTime?)Convert.ToDateTime(start) : null;
        }

        /// <summary>
        /// Gets the last activity time
        /// </summary>
        public static DateTime? GetLastActivityTime()
        {
            var last = HttpContext.Current.Session[LastActivityKey];
            return last != null ? (DateTime?)Convert.ToDateTime(last) : null;
        }

        /// <summary>
        /// Checks if the session has timed out
        /// </summary>
        public static bool IsSessionTimedOut(int timeoutMinutes = 20)
        {
            var lastActivity = GetLastActivityTime();
            if (!lastActivity.HasValue)
                return true;

            return (DateTime.Now - lastActivity.Value).TotalMinutes > timeoutMinutes;
        }

        /// <summary>
        /// Stores an object in session
        /// </summary>
        public static void Set(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        /// <summary>
        /// Retrieves an object from session
        /// </summary>
        public static object Get(string key)
        {
            return HttpContext.Current.Session[key];
        }

        /// <summary>
        /// Retrieves a typed object from session
        /// </summary>
        public static T Get<T>(string key) where T : class
        {
            return HttpContext.Current.Session[key] as T;
        }

        /// <summary>
        /// Removes an item from session
        /// </summary>
        public static void Remove(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }

        /// <summary>
        /// Checks if a session key exists
        /// </summary>
        public static bool Exists(string key)
        {
            return HttpContext.Current.Session[key] != null;
        }

        /// <summary>
        /// Gets the session ID
        /// </summary>
        public static string GetSessionId()
        {
            return HttpContext.Current.Session.SessionID;
        }
    }
}