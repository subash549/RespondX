using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class ManageScenarios : Page
    {
        private int moduleId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                if (int.TryParse(Request.QueryString["moduleId"], out moduleId))
                {
                    hfModuleID.Value = moduleId.ToString();
                    LoadModuleInfo();
                    LoadScenarios();
                }
                else
                {
                    Response.Redirect("ManageModules.aspx");
                }
            }
            else
            {
                moduleId = int.Parse(hfModuleID.Value);
            }
        }

        private void LoadModuleInfo()
        {
            var module = GetModuleById(moduleId);
            if (module != null)
            {
                lblModuleInfo.Text = $"Module: {module.Title}";
            }
        }

        private void LoadScenarios()
        {
            var scenarios = GetScenarios(moduleId);

            var difficulty = ddlDifficulty.SelectedValue;
            if (difficulty != "All")
            {
                int diff = int.Parse(difficulty);
                scenarios = scenarios.FindAll(s => s.DifficultyLevel == diff);
            }

            rptScenarios.DataSource = scenarios;
            rptScenarios.DataBind();
        }

        private ModuleItem GetModuleById(int id)
        {
            var modules = new List<ModuleItem>
            {
                new ModuleItem { ModuleID = 1, Title = "Introduction to Emergency Response" },
                new ModuleItem { ModuleID = 2, Title = "CPR and First Aid" }
            };
            return modules.Find(m => m.ModuleID == id);
        }

        private List<ScenarioItem> GetScenarios(int moduleId)
        {
            return new List<ScenarioItem>
            {
                new ScenarioItem { ScenarioID = 1, Title = "Office Emergency", Description = "Handle an office emergency", ScenarioOrder = 1, DifficultyLevel = 1, OptionsCount = 4, IsActive = true },
                new ScenarioItem { ScenarioID = 2, Title = "Home Fire", Description = "Respond to a home fire", ScenarioOrder = 2, DifficultyLevel = 3, OptionsCount = 4, IsActive = true }
            };
        }

        protected void rptScenarios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int scenarioId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Edit":
                    EditScenario(scenarioId);
                    break;
                case "Options":
                    ManageOptions(scenarioId);
                    break;
                case "Toggle":
                    ToggleScenario(scenarioId);
                    break;
                case "Delete":
                    DeleteScenario(scenarioId);
                    break;
            }
        }

        private void EditScenario(int scenarioId)
        {
            var scenario = GetScenarioById(scenarioId);
            if (scenario != null)
            {
                lblModalTitle.Text = "Edit Scenario";
                hfScenarioID.Value = scenarioId.ToString();
                txtTitle.Text = scenario.Title;
                txtDescription.Text = scenario.Description;
                txtScenarioText.Text = scenario.ScenarioText;
                txtOrder.Text = scenario.ScenarioOrder.ToString();
                ddlDifficultyLevel.SelectedValue = scenario.DifficultyLevel.ToString();
                chkIsActive.Checked = scenario.IsActive;

                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalScenario').modal('show');", true);
            }
        }

        private void ManageOptions(int scenarioId)
        {
            hfOptionsScenarioID.Value = scenarioId.ToString();
            LoadOptions(scenarioId);
            ScriptManager.RegisterStartupScript(this, GetType(), "showOptionsModal", "$('#modalOptions').modal('show');", true);
        }

        private void LoadOptions(int scenarioId)
        {
            var options = GetOptions(scenarioId);
            rptOptions.DataSource = options;
            rptOptions.DataBind();
        }

        private List<ScenarioOptionItem> GetOptions(int scenarioId)
        {
            return new List<ScenarioOptionItem>
            {
                new ScenarioOptionItem { OptionID = 1, OptionText = "Call 911 immediately", IsCorrect = false, OptionOrder = 1 },
                new ScenarioOptionItem { OptionID = 2, OptionText = "Check for responsiveness and begin CPR", IsCorrect = true, OptionOrder = 2 },
                new ScenarioOptionItem { OptionID = 3, OptionText = "Move the person to a comfortable position", IsCorrect = false, OptionOrder = 3 }
            };
        }

        private void ToggleScenario(int scenarioId)
        {
            ShowSuccess("Scenario status updated successfully");
            LoadScenarios();
        }

        private void DeleteScenario(int scenarioId)
        {
            ShowSuccess("Scenario deleted successfully");
            LoadScenarios();
        }

        protected void btnSaveScenario_Click(object sender, EventArgs e)
        {
            int scenarioId;
            bool isNew = !int.TryParse(hfScenarioID.Value, out scenarioId) || scenarioId == 0;

            if (string.IsNullOrEmpty(txtTitle.Text) || string.IsNullOrEmpty(txtScenarioText.Text))
            {
                ShowError("Title and scenario text are required.");
                return;
            }

            ShowSuccess(isNew ? "Scenario added successfully" : "Scenario updated successfully");
            ClearForm();
            LoadScenarios();

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#modalScenario').modal('hide');", true);
        }

        protected void btnAddOption_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOptionText.Text))
            {
                ShowError("Option text is required.");
                return;
            }

            int scenarioId = int.Parse(hfOptionsScenarioID.Value);
            ShowSuccess("Option added successfully");
            txtOptionText.Text = "";
            chkIsCorrectOption.Checked = false;
            LoadOptions(scenarioId);
        }

        protected void rptOptions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int optionId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "EditOption":
                    break;
                case "DeleteOption":
                    ShowSuccess("Option deleted successfully");
                    int scenarioId = int.Parse(hfOptionsScenarioID.Value);
                    LoadOptions(scenarioId);
                    break;
            }
        }

        private ScenarioItem GetScenarioById(int scenarioId)
        {
            var scenarios = GetScenarios(moduleId);
            return scenarios.Find(s => s.ScenarioID == scenarioId);
        }

        private void ClearForm()
        {
            hfScenarioID.Value = "";
            txtTitle.Text = "";
            txtDescription.Text = "";
            txtScenarioText.Text = "";
            txtOrder.Text = "";
            ddlDifficultyLevel.SelectedIndex = 0;
            chkIsActive.Checked = true;
            lblModalTitle.Text = "Add Scenario";
        }

        protected void btnAddScenario_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#modalScenario').modal('show');", true);
        }

        protected void ddlDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadScenarios();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageModules.aspx");
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