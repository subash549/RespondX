using System;
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

            LoadAlerts();
        }

        private void LoadAlerts()
        {
            var userId = SessionHelper.GetCurrentUserId();
            if (!userId.HasValue) return;
            rptAlerts.DataSource = AlertRepository.GetLearnerAlerts(userId.Value);
            rptAlerts.DataBind();
        }

        protected void rptAlerts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int alertId;
            var userId = SessionHelper.GetCurrentUserId();
            if (!userId.HasValue || !int.TryParse(Convert.ToString(e.CommandArgument), out alertId))
                return;

            try
            {
                switch (e.CommandName)
                {
                    case "Acknowledge":
                        AlertRepository.LearnerAction(userId.Value, alertId, "Acknowledge");
                        ShowNotification("Alert acknowledged.", "success");
                        break;
                    case "Dismiss":
                        AlertRepository.LearnerAction(userId.Value, alertId, "Dismiss");
                        ShowNotification("Alert dismissed.", "info");
                        break;
                }
                LoadAlerts();
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Update learner alert", ex.Message, ex.StackTrace);
                ShowNotification("The alert could not be updated. Please try again.", "error");
                LoadAlerts();
            }
        }

        protected void btnMarkAllRead_Click(object sender, EventArgs e)
        {
            PerformBulkAction("Read", "All alerts marked as read.");
        }

        protected void btnClearAll_Click(object sender, EventArgs e)
        {
            PerformBulkAction("Dismiss", "All alerts cleared.");
        }

        private void PerformBulkAction(string action, string successMessage)
        {
            var userId = SessionHelper.GetCurrentUserId();
            if (!userId.HasValue) return;
            try
            {
                int affected = AlertRepository.LearnerAction(userId.Value, null, action);
                ShowNotification(affected == 0 ? "There are no alerts to update." : successMessage, "success");
                LoadAlerts();
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Bulk update learner alerts", ex.Message, ex.StackTrace);
                ShowNotification("Alerts could not be updated. Please try again.", "error");
                LoadAlerts();
            }
        }

        private void ShowNotification(string message, string type)
        {
            UiHelper.Notify(this, message, type);
        }
    }

}
