using System;
using System.Collections.Generic;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class Scenarios : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                LoadScenarios();
            }
        }

        private void LoadScenarios()
        {
            var scenarios = GetScenarios();

            var difficulty = ddlDifficulty.SelectedValue;
            if (difficulty != "All")
            {
                scenarios = scenarios.FindAll(s => s.Difficulty == difficulty);
            }

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                scenarios = scenarios.FindAll(s =>
                    s.Title.ToLower().Contains(search.ToLower()) ||
                    s.Description.ToLower().Contains(search.ToLower())
                );
            }

            rptScenarios.DataSource = scenarios;
            rptScenarios.DataBind();
        }

        private List<ScenarioItem> GetScenarios()
        {
            return new List<ScenarioItem>
            {
                new ScenarioItem
                {
                    ScenarioID = 1,
                    Title = "Office Emergency Response",
                    Description = "Handle a medical emergency in an office environment with multiple people involved.",
                    Module = "Emergency Response",
                    Difficulty = "Beginner",
                    DifficultyClass = "beginner",
                    TimeEstimate = 15,
                    Status = "Not Started",
                    StatusBadge = "badge-secondary",
                    Attempts = 0
                },
                new ScenarioItem
                {
                    ScenarioID = 2,
                    Title = "Home Fire Emergency",
                    Description = "Respond to a fire emergency in a residential setting with family members.",
                    Module = "Fire Safety",
                    Difficulty = "Intermediate",
                    DifficultyClass = "intermediate",
                    TimeEstimate = 20,
                    Status = "In Progress",
                    StatusBadge = "badge-warning",
                    Attempts = 2
                },
                new ScenarioItem
                {
                    ScenarioID = 3,
                    Title = "Natural Disaster Response",
                    Description = "Coordinate emergency response for a natural disaster affecting a community.",
                    Module = "Disaster Management",
                    Difficulty = "Advanced",
                    DifficultyClass = "advanced",
                    TimeEstimate = 30,
                    Status = "Completed",
                    StatusBadge = "badge-success",
                    Attempts = 3
                }
            };
        }

        protected void ddlDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadScenarios();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadScenarios();
        }
    }
}