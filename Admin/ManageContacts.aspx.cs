using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageContacts : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                LoadLearners();
                LoadContacts();
            }
        }

        private void LoadLearners()
        {
            var learners = new List<LearnerItem>
            {
                new LearnerItem { LearnerID = 1, Name = "John Smith" },
                new LearnerItem { LearnerID = 2, Name = "Jane Doe" }
            };

            ddlLearner.DataSource = learners;
            ddlLearner.DataTextField = "Name";
            ddlLearner.DataValueField = "LearnerID";
            ddlLearner.DataBind();
            ddlLearner.Items.Insert(0, new ListItem("All Learners", "All"));
        }

        private void LoadContacts()
        {
            var contacts = GetContacts();

            var learnerId = ddlLearner.SelectedValue;
            if (learnerId != "All")
            {
                int id = int.Parse(learnerId);
                contacts = contacts.FindAll(c => c.LearnerID == id);
            }

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                contacts = contacts.FindAll(c =>
                    c.FullName.ToLower().Contains(search.ToLower()) ||
                    c.LearnerName.ToLower().Contains(search.ToLower()) ||
                    c.PhoneNumber.Contains(search)
                );
            }

            rptContacts.DataSource = contacts;
            rptContacts.DataBind();
        }

        private List<ContactItem> GetContacts()
        {
            return new List<ContactItem>
            {
                new ContactItem { ContactID = 1, LearnerID = 1, LearnerName = "John Smith", FullName = "Mary Smith", Relationship = "Spouse", PhoneNumber = "(555) 123-4567", Email = "mary@email.com", IsPrimary = true },
                new ContactItem { ContactID = 2, LearnerID = 1, LearnerName = "John Smith", FullName = "Bob Johnson", Relationship = "Brother", PhoneNumber = "(555) 987-6543", Email = "bob@email.com", IsPrimary = false },
                new ContactItem { ContactID = 3, LearnerID = 2, LearnerName = "Jane Doe", FullName = "Tom Doe", Relationship = "Husband", PhoneNumber = "(555) 456-7890", Email = "tom@email.com", IsPrimary = true }
            };
        }

        protected void rptContacts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int contactId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Edit":
                    ShowSuccess("Edit functionality would open here");
                    break;
                case "Delete":
                    DeleteContact(contactId);
                    break;
            }
        }

        private void DeleteContact(int contactId)
        {
            ShowSuccess("Contact deleted successfully");
            LoadContacts();
        }

        protected void ddlLearner_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadContacts();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadContacts();
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

    public class LearnerItem
    {
        public int LearnerID { get; set; }
        public string Name { get; set; }
    }
}