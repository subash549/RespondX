using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;

namespace RespondX
{
    public partial class Notifications : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireAuthentication()) return;
            if (!IsPostBack) LoadNotifications();
        }

        private void LoadNotifications()
        {
            int? userId = SessionHelper.GetCurrentUserId();
            if (!userId.HasValue) return;
            var items = NotificationRepository.GetRecent(userId.Value, 50);
            rptNotifications.DataSource = items;
            rptNotifications.DataBind();
            pnlEmpty.Visible = items.Count == 0;
        }

        protected void rptNotifications_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            int? userId = SessionHelper.GetCurrentUserId();
            if (e.CommandName != "Open" || !userId.HasValue || !int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            NotificationRepository.MarkRead(userId.Value, id);
            var item = NotificationRepository.GetRecent(userId.Value, 50).Find(n => n.NotificationID == id);
            if (item != null && !string.IsNullOrWhiteSpace(item.TargetUrl))
            {
                string target = item.TargetUrl;
                if (target.StartsWith("~/") || (target.StartsWith("/") && !target.StartsWith("//")))
                {
                    Response.Redirect(ResolveUrl(target), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }
            LoadNotifications();
        }
    }
}
