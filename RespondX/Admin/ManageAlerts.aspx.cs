using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageAlerts : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                LoadAlerts();
            }
        }

        private void LoadAlerts()
        {
            var alerts = GetAlerts();

            var type = ddlType.SelectedValue;
            if (type != "All")
            {
                alerts = alerts.FindAll(a => a.AlertType == type);
            }

            var status = ddlStatus.SelectedValue;
            if (status != "All")
            {
                bool isActive = status == "Active";
                alerts = alerts.FindAll(a => a.IsActive == isActive);
            }

            rptAlerts.DataSource = alerts;
            rptAlerts.DataBind();
        }

        private List<AlertItem> GetAlerts()
        {
            return new List<AlertItem>
            {
                new AlertItem
                {
                    AlertID = 1,
                    Title = "System Maintenance",
                    Message = "Scheduled maintenance this weekend. System may be unavailable.",
                    AlertType = "System",
                    AlertTypeDisplay = "System",
                    TypeBadge = "badge-secondary",
                    Priority = 3,
                    PriorityLevel = "Medium",
                    PriorityClass = "medium",
                    Target = "All Users",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-1),
                    TimeAgo = "1 day ago"
                },
                new AlertItem
                {
                    AlertID = 2,
                    Title = "CPR Certification Deadline",
                    Message = "Several learners have certifications expiring soon.",
                    AlertType = "Reminder",
                    AlertTypeDisplay = "Reminder",
                    TypeBadge = "badge-warning",
                    Priority = 4,
                    PriorityLevel = "High",
                    PriorityClass = "high",
                    Target = "Learners",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-3),
                    TimeAgo = "3 days ago"
                }
            };
        }

        protected void rptAlerts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int alertId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Edit":
                    EditAlert(alertId);
                    break;
                case "Delete":
                    DeleteAlert(alertId);
                    break;
            }
        }

        private void EditAlert(int alertId)
        {
            var alert = GetAlertById(alertId);
            if (alert != null)
            {
                lblModalTitle.Text = "Edit Alert";
                hfAlertID.Value = alertId.ToString();
                txtTitle.Text = alert.Title;
                txtMessage.Text = alert.Message;
                ddlAlertType.SelectedValue = alert.AlertType;
                ddlPriority.SelectedValue = alert.Priority.ToString();
                ddlTarget.SelectedValue = alert.Target.Replace(" ", "");

                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalAlert').modal('show');", true);
            }
        }

        private void DeleteAlert(int alertId)
        {
            ShowSuccess("Alert deleted successfully");
            LoadAlerts();
        }

        protected void btnSaveAlert_Click(object sender, EventArgs e)
        {
            int alertId;
            bool isNew = !int.TryParse(hfAlertID.Value, out alertId) || alertId == 0;

            if (string.IsNullOrEmpty(txtTitle.Text) || string.IsNullOrEmpty(txtMessage.Text))
            {
                ShowError("Title and message are required.");
                return;
            }

            ShowSuccess(isNew ? "Alert created successfully" : "Alert updated successfully");
            ClearForm();
            LoadAlerts();

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalAlert').modal('hide');", true);
        }

        private AlertItem GetAlertById(int alertId)
        {
            var alerts = GetAlerts();
            return alerts.Find(a => a.AlertID == alertId);
        }

        private void ClearForm()
        {
            hfAlertID.Value = "";
            txtTitle.Text = "";
            txtMessage.Text = "";
            ddlAlertType.SelectedIndex = 0;
            ddlPriority.SelectedIndex = 2;
            ddlTarget.SelectedIndex = 0;
            ddlExpires.SelectedIndex = 0;
            lblModalTitle.Text = "Create Alert";
        }

        protected void btnAddAlert_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalAlert').modal('show');", true);
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAlerts();
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAlerts();
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            lblSuccess.Text = message;
            pnlError.Visible = false;
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
            pnlSuccess.Visible = false;
        }
    }
}