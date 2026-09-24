using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;

namespace RespondX.Learner
{
    public partial class Alerts : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                LoadAlerts();
            }
        }

        private void LoadAlerts()
        {
            var userId = SessionHelper.GetCurrentUserId();
            if (!userId.HasValue) return;

            // In production, load from database
            var alerts = new List<AlertItem>
            {
                new AlertItem
                {
                    AlertID = 1,
                    Title = "New Module Available",
                    Message = "Advanced Emergency Response module has been added to your learning path.",
                    AlertType = "Notification",
                    TypeBadge = "badge-info",
                    AlertTypeDisplay = "Notification",
                    Priority = 2,
                    PriorityLevel = "Low",
                    PriorityClass = "low",
                    IsRead = false,
                    IsAcknowledged = false,
                    CreatedAt = DateTime.Now.AddHours(-2),
                    TimeAgo = "2 hours ago"
                },
                new AlertItem
                {
                    AlertID = 2,
                    Title = "CPR Certification Reminder",
                    Message = "Your CPR certification will expire in 30 days. Complete the refresher course.",
                    AlertType = "Reminder",
                    TypeBadge = "badge-warning",
                    AlertTypeDisplay = "Reminder",
                    Priority = 4,
                    PriorityLevel = "High",
                    PriorityClass = "high",
                    IsRead = false,
                    IsAcknowledged = false,
                    CreatedAt = DateTime.Now.AddDays(-1),
                    TimeAgo = "1 day ago"
                },
                new AlertItem
                {
                    AlertID = 3,
                    Title = "Emergency Alert",
                    Message = "Severe weather warning in your area. Please review emergency procedures.",
                    AlertType = "Emergency",
                    TypeBadge = "badge-danger",
                    AlertTypeDisplay = "Emergency",
                    Priority = 5,
                    PriorityLevel = "Critical",
                    PriorityClass = "critical",
                    IsRead = false,
                    IsAcknowledged = false,
                    CreatedAt = DateTime.Now.AddDays(-2),
                    TimeAgo = "2 days ago"
                }
            };

            rptAlerts.DataSource = alerts;
            rptAlerts.DataBind();
        }

        protected void rptAlerts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int alertId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Acknowledge":
                    AcknowledgeAlert(alertId);
                    break;
                case "Dismiss":
                    DismissAlert(alertId);
                    break;
            }
        }

        private void AcknowledgeAlert(int alertId)
        {
            // In production, update database
            ShowNotification("Alert acknowledged", "success");
            LoadAlerts();
        }

        private void DismissAlert(int alertId)
        {
            // In production, update database
            ShowNotification("Alert dismissed", "info");
            LoadAlerts();
        }

        protected void btnMarkAllRead_Click(object sender, EventArgs e)
        {
            // In production, update database
            ShowNotification("All alerts marked as read", "success");
            LoadAlerts();
        }

        protected void btnClearAll_Click(object sender, EventArgs e)
        {
            // In production, clear from database
            ShowNotification("All alerts cleared", "info");
            LoadAlerts();
        }

        private void ShowNotification(string message, string type)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "notify",
                $"showNotification('{message}', '{type}');", true);
        }
    }

    public class AlertItem
    {
        public int AlertID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string AlertType { get; set; }
        public string TypeBadge { get; set; }
        public string AlertTypeDisplay { get; set; }
        public int Priority { get; set; }
        public string PriorityLevel { get; set; }
        public string PriorityClass { get; set; }
        public bool IsRead { get; set; }
        public bool IsAcknowledged { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; }
    }
}