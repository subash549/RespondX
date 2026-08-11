using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class ScenarioPractice : Page
    {
        private int scenarioId;
        private bool isAnswered = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                if (int.TryParse(Request.QueryString["id"], out scenarioId))
                {
                    LoadScenario(scenarioId);
                }
                else
                {
                    pnlScenario.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadScenario(int id)
        {
            var scenario = GetScenario(id);
            if (scenario == null)
            {
                pnlScenario.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            pnlScenario.Visible = true;
            lblTitle.Text = scenario.Title;
            lblDifficulty.Text = scenario.Difficulty;
            lblDescription.Text = scenario.Description;
            litScenarioText.Text = scenario.ScenarioText;
            lblProgress.Text = "0%";
            lblProgressBar.Text = "0";

            LoadOptions(id);
            btnNextScenario.Visible = false;
        }

        private ScenarioItem GetScenario(int id)
        {
            var scenarios = new Dictionary<int, ScenarioItem>
            {
                { 1, new ScenarioItem { ScenarioID = 1, Title = "Office Medical Emergency", Difficulty = "Beginner", Description = "A colleague collapses in the office", ScenarioText = "<p>You are working in an office when a colleague suddenly collapses. They are unresponsive and not breathing.</p><p><strong>What is your immediate response?</strong></p>" } }
            };

            return scenarios.ContainsKey(id) ? scenarios[id] : null;
        }

        private void LoadOptions(int scenarioId)
        {
            var options = new List<ScenarioOptionItem>
            {
                new ScenarioOptionItem { OptionID = 1, OptionText = "Call 911 immediately and wait for help", IsCorrect = false, Feedback = "While calling 911 is important, you should also provide immediate assistance." },
                new ScenarioOptionItem { OptionID = 2, OptionText = "Check for responsiveness, call for help, and begin CPR", IsCorrect = true, Feedback = "Correct! Checking responsiveness, calling for help, and beginning CPR are the right steps." },
                new ScenarioOptionItem { OptionID = 3, OptionText = "Move the person to a comfortable position", IsCorrect = false, Feedback = "Moving an unresponsive person could cause further injury. You should not move them." },
                new ScenarioOptionItem { OptionID = 4, OptionText = "Wait for someone else to take action", IsCorrect = false, Feedback = "In an emergency, immediate action is crucial. Don't wait for others to act." }
            };

            rptOptions.DataSource = options;
            rptOptions.DataBind();
        }

        protected void rptOptions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // Handle option selection if needed
        }

        protected void btnSubmitAnswer_Click(object sender, EventArgs e)
        {
            if (isAnswered) return;

            var selectedOption = Request.Form["scenarioOption"];
            if (string.IsNullOrEmpty(selectedOption))
            {
                ShowNotification("Please select an option first.", "warning");
                return;
            }

            int optionId = int.Parse(selectedOption);
            var option = GetOptionById(optionId);

            if (option == null) return;

            isAnswered = true;
            btnSubmitAnswer.Enabled = false;
            btnNextScenario.Visible = true;

            var optionElement = $".scenario-option input[value='{optionId}']";
            var script = $"var element = document.querySelector('{optionElement}').closest('.scenario-option'); showFeedback(element, {option.IsCorrect.ToString().ToLower()}, '{option.Feedback.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "feedback", script, true);

            lblProgress.Text = "100%";
            lblProgressBar.Text = "100";
        }

        private ScenarioOptionItem GetOptionById(int optionId)
        {
            var options = new List<ScenarioOptionItem>
            {
                new ScenarioOptionItem { OptionID = 1, IsCorrect = false, Feedback = "While calling 911 is important, you should also provide immediate assistance." },
                new ScenarioOptionItem { OptionID = 2, IsCorrect = true, Feedback = "Correct! Checking responsiveness, calling for help, and beginning CPR are the right steps." },
                new ScenarioOptionItem { OptionID = 3, IsCorrect = false, Feedback = "Moving an unresponsive person could cause further injury." },
                new ScenarioOptionItem { OptionID = 4, IsCorrect = false, Feedback = "In an emergency, immediate action is crucial." }
            };

            return options.Find(o => o.OptionID == optionId);
        }

        protected void btnNextScenario_Click(object sender, EventArgs e)
        {
            Response.Redirect("Scenarios.aspx");
        }

        private void ShowNotification(string message, string type)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "notify",
                $"showNotification('{message}', '{type}');", true);
        }
    }
}