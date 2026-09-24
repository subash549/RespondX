using System;
using System.Web;
using RespondX.Models;

namespace RespondX.Helpers
{
    public static class AuthorizationHelper
    {
        /// <summary>
        /// Checks if the current user is authorized for a specific role
        /// </summary>
        public static bool IsAuthorized(string requiredRole)
        {
            if (!SessionHelper.IsUserLoggedIn())
                return false;

            var user = SessionHelper.GetCurrentUser();
            if (user == null)
                return false;

            return string.Equals(user.Role, requiredRole, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if the current user is authorized for any of the specified roles
        /// </summary>
        public static bool IsAuthorizedAny(params string[] roles)
        {
            if (!SessionHelper.IsUserLoggedIn())
                return false;

            var user = SessionHelper.GetCurrentUser();
            if (user == null)
                return false;

            foreach (var role in roles)
            {
                if (string.Equals(user.Role, role, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the current user is an admin
        /// </summary>
        public static bool IsAdmin()
        {
            return IsAuthorized("Admin");
        }

        /// <summary>
        /// Checks if the current user is an expert
        /// </summary>
        public static bool IsExpert()
        {
            return IsAuthorized("Expert");
        }

        /// <summary>
        /// Checks if the current user is a learner
        /// </summary>
        public static bool IsLearner()
        {
            return IsAuthorized("Learner");
        }

        /// <summary>
        /// Ensures the user is authenticated, redirects to login if not
        /// </summary>
        public static bool RequireAuthentication()
        {
            if (!SessionHelper.IsUserLoggedIn())
            {
                var request = HttpContext.Current.Request;
                string returnUrl = request.AppRelativeCurrentExecutionFilePath;
                if (!string.IsNullOrEmpty(request.QueryString.ToString()))
                {
                    returnUrl += "?" + request.QueryString;
                }
                
                HttpContext.Current.Response.Redirect("~/Login.aspx?returnUrl=" + HttpContext.Current.Server.UrlEncode(returnUrl));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Ensures the user has the required role, redirects if not
        /// </summary>
        public static bool RequireRole(string requiredRole)
        {
            if (!RequireAuthentication())
                return false;

            if (!IsAuthorized(requiredRole))
            {
                HttpContext.Current.Response.Redirect("~/Unauthorized.aspx");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Ensures the user has any of the required roles, redirects if not
        /// </summary>
        public static bool RequireAnyRole(params string[] roles)
        {
            if (!RequireAuthentication())
                return false;

            if (!IsAuthorizedAny(roles))
            {
                HttpContext.Current.Response.Redirect("~/Unauthorized.aspx");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the current user owns a resource (by user ID)
        /// </summary>
        public static bool IsResourceOwner(int resourceOwnerId)
        {
            var userId = SessionHelper.GetCurrentUserId();
            if (!userId.HasValue)
                return false;

            return userId.Value == resourceOwnerId;
        }

        /// <summary>
        /// Checks if the current user can access a resource (owner or admin)
        /// </summary>
        public static bool CanAccessResource(int resourceOwnerId)
        {
            return IsAdmin() || IsResourceOwner(resourceOwnerId);
        }

        /// <summary>
        /// Gets the appropriate dashboard URL for the current user
        /// </summary>
        public static string GetDashboardUrl()
        {
            if (!SessionHelper.IsUserLoggedIn())
                return "~/Login.aspx";

            var role = SessionHelper.GetUserRole();

            switch (role?.ToLower())
            {
                case "admin":
                    return "~/Admin/AdminDashboard.aspx";
                case "expert":
                    return "~/Expert/ExpertDashboard.aspx";
                case "learner":
                    return "~/Learner/LearnerDashboard.aspx";
                default:
                    return "~/Login.aspx";
            }
        }

        /// <summary>
        /// Redirects to the appropriate dashboard
        /// </summary>
        public static void RedirectToDashboard()
        {
            var url = GetDashboardUrl();
            HttpContext.Current.Response.Redirect(url);
        }

        /// <summary>
        /// Validates user permissions for a specific action
        /// </summary>
        public static bool HasPermission(string permission, User user = null)
        {
            if (user == null)
            {
                if (!SessionHelper.IsUserLoggedIn())
                    return false;
                user = SessionHelper.GetCurrentUser();
            }

            if (user == null)
                return false;

            // Admins have all permissions
            if (user.Role == "Admin")
                return true;

            // Check specific permissions based on role
            switch (permission.ToLower())
            {
                case "view_dashboard":
                    return true;
                case "manage_users":
                    return user.Role == "Admin";
                case "manage_content":
                    return user.Role == "Admin" || user.Role == "Expert";
                case "review_content":
                    return user.Role == "Expert";
                case "take_quizzes":
                    return user.Role == "Learner";
                case "view_reports":
                    return user.Role == "Admin" || user.Role == "Expert";
                default:
                    return false;
            }
        }
    }
}