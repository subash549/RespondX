using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;

namespace RespondX.Admin
{
    public partial class ManageAlerts : Page
    {
        protected override void OnInit(EventArgs e)
        {
            BindRecipients();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
                LoadAlerts();
        }

        private void BindRecipients()
        {
            string selectedValue = ddlTarget.SelectedValue;
            ddlTarget.Items.Clear();
            ddlTarget.Items.Add(new ListItem("All active learners", "All"));
            ddlTarget.Items.Add(new ListItem("Select a learner", ""));
            foreach (var learner in AlertRepository.GetActiveLearners())
                ddlTarget.Items.Add(new ListItem(learner.Text, learner.Value));

            if (!string.IsNullOrEmpty(selectedValue) && ddlTarget.Items.FindByValue(selectedValue) != null)
                ddlTarget.SelectedValue = selectedValue;
        }

        private void LoadAlerts()
        {
            try
            {
                rptAlerts.DataSource = AlertRepository.GetAlertGroups(
                    ddlType.SelectedValue == "All" ? null : ddlType.SelectedValue,
                    ddlStatus.SelectedValue == "All" ? null : ddlStatus.SelectedValue);
                rptAlerts.DataBind();
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Load alert groups", ex.Message, ex.StackTrace);
                rptAlerts.DataSource = new object[0];
                rptAlerts.DataBind();
                ShowError("Alerts could not be loaded. Check the database connection and confirm the alerts schema is applied.");
            }
        }

        protected void rptAlerts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int alertId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out alertId) || alertId <= 0)
            {
                ShowError("Select a valid alert.");
                return;
            }

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
            try
            {
                var alert = AlertRepository.GetAlert(alertId);
                if (alert == null)
                {
                    ShowError("That alert no longer exists.");
                    LoadAlerts();
                    return;
                }

                lblModalTitle.Text = "Edit Alert";
                hfAlertID.Value = alertId.ToString();
                txtTitle.Text = alert.Title;
                txtMessage.Text = alert.Message;
                ddlAlertType.SelectedValue = alert.AlertType;
                ddlPriority.SelectedValue = alert.Priority.ToString();
                int? recipientId = AlertRepository.GetAlertRecipient(alertId);
                ddlTarget.SelectedValue = recipientId.HasValue ? recipientId.Value.ToString() : "All";
                ddlExpires.SelectedValue = ExpirySelection(alert.ExpiresAt);
                ShowModal();
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Load alert for editing", ex.Message, ex.StackTrace);
                ShowError("The alert could not be loaded for editing.");
            }
        }

        private void DeleteAlert(int alertId)
        {
            try
            {
                AlertRepository.DeleteAlertGroup(alertId);
                ShowSuccess("Alert deleted successfully.");
                LoadAlerts();
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Delete alert group", ex.Message, ex.StackTrace);
                ShowError("The alert could not be deleted.");
            }
        }

        protected void btnSaveAlert_Click(object sender, EventArgs e)
        {
            int alertId;
            bool isNew = !int.TryParse(hfAlertID.Value, out alertId) || alertId <= 0;
            string title = txtTitle.Text.Trim();
            string message = txtMessage.Text.Trim();
            int priority;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message) || title.Length > 100 || message.Length > 4000)
            {
                ShowError("Enter a title (up to 100 characters) and message (up to 4,000 characters).");
                ShowModal();
                return;
            }
            if (!int.TryParse(ddlPriority.SelectedValue, out priority) || priority < 1 || priority > 5)
            {
                ShowError("Select a valid priority.");
                ShowModal();
                return;
            }
            if (Array.IndexOf(AlertRepository.AlertTypes, ddlAlertType.SelectedValue) < 0)
            {
                ShowError("Select a valid alert type.");
                ShowModal();
                return;
            }

            int recipientId = 0;
            DateTime? expiry = CalculateExpiry(ddlExpires.SelectedValue);
            try
            {
                if (isNew)
                {
                    if (string.IsNullOrEmpty(ddlTarget.SelectedValue) ||
                        (ddlTarget.SelectedValue != "All" && !int.TryParse(ddlTarget.SelectedValue, out recipientId)))
                    {
                        ShowError("Select a valid recipient.");
                        ShowModal();
                        return;
                    }

                    int recipients = AlertRepository.CreateAlert(title, message, ddlAlertType.SelectedValue,
                        priority, expiry, recipientId == 0 ? (int?)null : recipientId,
                        SessionHelper.GetCurrentUserId() ?? 0);
                    if (recipients == 0)
                    {
                        ShowError("No active learner matched the selected audience.");
                        ShowModal();
                        return;
                    }
                    ShowSuccess("Alert sent to " + recipients + (recipients == 1 ? " learner." : " learners."));
                }
                else
                {
                    if (string.IsNullOrEmpty(ddlTarget.SelectedValue) ||
                        (ddlTarget.SelectedValue != "All" && !int.TryParse(ddlTarget.SelectedValue, out recipientId)))
                    {
                        ShowError("Select a valid recipient.");
                        ShowModal();
                        return;
                    }
                    int updated = AlertRepository.UpdateAlertGroup(alertId, title, message,
                        ddlAlertType.SelectedValue, priority, expiry, true, recipientId == 0 ? (int?)null : recipientId);
                    if (updated == 0)
                    {
                        ShowError("That alert no longer exists. Refresh the list and try again.");
                        LoadAlerts();
                        return;
                    }
                    ShowSuccess("Alert updated for " + updated + (updated == 1 ? " recipient." : " recipients."));
                }

                ClearForm();
                LoadAlerts();
                ScriptManager.RegisterStartupScript(this, GetType(), "hideAlertModal", "$('#modalAlert').modal('hide');", true);
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Save alert", ex.Message, ex.StackTrace);
                ShowError("The alert could not be saved. Check the database connection and try again.");
                ShowModal();
            }
        }

        private void ClearForm()
        {
            hfAlertID.Value = string.Empty;
            txtTitle.Text = string.Empty;
            txtMessage.Text = string.Empty;
            ddlAlertType.SelectedIndex = 0;
            ddlPriority.SelectedValue = "3";
            ddlTarget.SelectedValue = "All";
            ddlExpires.SelectedValue = "0";
            lblModalTitle.Text = "Create Alert";
        }

        protected void btnAddAlert_Click(object sender, EventArgs e)
        {
            ClearForm();
            ShowModal();
        }

        private void ShowModal()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showAlertModal", "$('#modalAlert').modal('show');", true);
        }

        private static DateTime? CalculateExpiry(string daysValue)
        {
            int days;
            return int.TryParse(daysValue, out days) && days > 0 ? DateTime.Now.AddDays(days) : (DateTime?)null;
        }

        private static string ExpirySelection(DateTime? expiresAt)
        {
            if (!expiresAt.HasValue) return "0";
            int remainingDays = (int)Math.Ceiling((expiresAt.Value - DateTime.Now).TotalDays);
            if (remainingDays <= 1) return "1";
            if (remainingDays <= 7) return "7";
            if (remainingDays <= 30) return "30";
            return "90";
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
            lblSuccess.Text = Server.HtmlEncode(message);
            pnlError.Visible = false;
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = Server.HtmlEncode(message);
            pnlSuccess.Visible = false;
        }
    }
}
