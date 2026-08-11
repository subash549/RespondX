using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageModules : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                LoadModules();
            }
        }

        private void LoadModules()
        {
            var modules = GetModules();

            var status = ddlStatus.SelectedValue;
            if (status != "All")
            {
                bool isActive = status == "Active";
                modules = modules.FindAll(m => m.IsActive == isActive);
            }

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                modules = modules.FindAll(m =>
                    m.Title.ToLower().Contains(search.ToLower()) ||
                    m.Description.ToLower().Contains(search.ToLower())
                );
            }

            rptModules.DataSource = modules;
            rptModules.DataBind();
        }

        private List<ModuleItem> GetModules()
        {
            return new List<ModuleItem>
            {
                new ModuleItem { ModuleID = 1, Title = "Introduction to Emergency Response", Description = "Learn the fundamentals of emergency response", LessonCount = 5, EstimatedHours = 2, IsActive = true, CreatedAt = DateTime.Now.AddMonths(-1) },
                new ModuleItem { ModuleID = 2, Title = "CPR and First Aid", Description = "Comprehensive CPR and first aid training", LessonCount = 8, EstimatedHours = 4, IsActive = true, CreatedAt = DateTime.Now.AddDays(-15) },
                new ModuleItem { ModuleID = 3, Title = "Emergency Communication", Description = "Effective communication during emergencies", LessonCount = 6, EstimatedHours = 3, IsActive = false, CreatedAt = DateTime.Now.AddDays(-5) }
            };
        }

        protected void rptModules_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int moduleId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Edit":
                    EditModule(moduleId);
                    break;
                case "Lessons":
                    Response.Redirect($"ManageLessons.aspx?moduleId={moduleId}");
                    break;
                case "Toggle":
                    ToggleModule(moduleId);
                    break;
                case "Delete":
                    DeleteModule(moduleId);
                    break;
            }
        }

        private void EditModule(int moduleId)
        {
            var module = GetModuleById(moduleId);
            if (module != null)
            {
                lblModalTitle.Text = "Edit Module";
                hfModuleID.Value = moduleId.ToString();
                txtTitle.Text = module.Title;
                txtDescription.Text = module.Description;
                txtOrder.Text = module.ModuleOrder.ToString();
                txtHours.Text = module.EstimatedHours.ToString();
                chkIsMandatory.Checked = module.IsMandatory;
                chkIsActive.Checked = module.IsActive;

                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalModule').modal('show');", true);
            }
        }

        private void ToggleModule(int moduleId)
        {
            ShowSuccess("Module status updated successfully");
            LoadModules();
        }

        private void DeleteModule(int moduleId)
        {
            ShowSuccess("Module deleted successfully");
            LoadModules();
        }

        protected void btnSaveModule_Click(object sender, EventArgs e)
        {
            int moduleId;
            bool isNew = !int.TryParse(hfModuleID.Value, out moduleId) || moduleId == 0;

            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                ShowError("Title is required.");
                return;
            }

            ShowSuccess(isNew ? "Module added successfully" : "Module updated successfully");
            ClearForm();
            LoadModules();

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalModule').modal('hide');", true);
        }

        private ModuleItem GetModuleById(int moduleId)
        {
            var modules = GetModules();
            return modules.Find(m => m.ModuleID == moduleId);
        }

        private void ClearForm()
        {
            hfModuleID.Value = "";
            txtTitle.Text = "";
            txtDescription.Text = "";
            txtOrder.Text = "";
            txtHours.Text = "";
            chkIsMandatory.Checked = false;
            chkIsActive.Checked = true;
            lblModalTitle.Text = "Add Module";
        }

        protected void btnAddModule_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalModule').modal('show');", true);
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadModules();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadModules();
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