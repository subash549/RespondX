using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;

namespace RespondX.Learner
{
    public partial class EmergencyContacts : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                LoadContacts();
            }
        }

        private void LoadContacts()
        {
            var userId = SessionHelper.GetCurrentUserId();
            if (!userId.HasValue) return;

            // In production, load from database
            var contacts = new List<Contact>
            {
                new Contact { ContactID = 1, FullName = "Jane Smith", Relationship = "Spouse", PhoneNumber = "(555) 123-4567", Email = "jane@email.com", IsPrimary = true },
                new Contact { ContactID = 2, FullName = "Bob Johnson", Relationship = "Brother", PhoneNumber = "(555) 987-6543", Email = "bob@email.com", IsPrimary = false }
            };

            rptContacts.DataSource = contacts;
            rptContacts.DataBind();
        }

        protected void rptContacts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int contactId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Delete":
                    DeleteContact(contactId);
                    break;
                case "SetPrimary":
                    SetPrimaryContact(contactId);
                    break;
                case "Edit":
                    Response.Redirect($"EditContact.aspx?id={contactId}");
                    break;
            }
        }

        private void DeleteContact(int contactId)
        {
            // In production, delete from database
            ShowNotification("Contact deleted successfully", "success");
            LoadContacts();
        }

        private void SetPrimaryContact(int contactId)
        {
            // In production, update database
            ShowNotification("Primary contact updated", "success");
            LoadContacts();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // In production, save to database
                ShowNotification("Contact added successfully", "success");
                ClearForm();
                LoadContacts();

                // Close modal
                ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", "$('#modalAddContact').modal('hide');", true);
            }
        }

        private void ClearForm()
        {
            txtName.Text = "";
            txtRelationship.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            chkPrimary.Checked = false;
        }

        private void ShowNotification(string message, string type)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "notify",
                $"showNotification('{message}', '{type}');", true);
        }
    }

    public class Contact
    {
        public int ContactID { get; set; }
        public string FullName { get; set; }
        public string Relationship { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsPrimary { get; set; }
    }
}