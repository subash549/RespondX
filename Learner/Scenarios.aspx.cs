using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            var scenarios = new List<ScenarioItem>();
            var learnerId = SessionHelper.GetCurrentUserId().GetValueOrDefault();
            int difficulty;
            bool filterDifficulty = int.TryParse(ddlDifficulty.SelectedValue, out difficulty);
            string search = txtSearch.Text.Trim();

            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                using (var cmd = new SqlCommand(@"
                    SELECT s.ScenarioID, s.Title, s.Description, s.ScenarioText, s.DifficultyLevel, m.Title AS ModuleTitle,
                           (SELECT TOP 1 lp.Status FROM dbo.LearnerProgress lp
                            WHERE lp.LearnerID = @LearnerID AND lp.ScenarioID = s.ScenarioID
                            ORDER BY lp.LastAccessedAt DESC) AS LearnerStatus
                    FROM dbo.Scenarios s
                    INNER JOIN dbo.Modules m ON m.ModuleID = s.ModuleID
                    WHERE s.IsActive = 1 AND m.IsActive = 1
                      AND EXISTS (SELECT 1 FROM dbo.ScenarioOptions o WHERE o.ScenarioID = s.ScenarioID)
                      AND (@Difficulty IS NULL OR s.DifficultyLevel = @Difficulty)
                      AND (@Search IS NULL OR s.Title LIKE @Search OR s.Description LIKE @Search OR m.Title LIKE @Search)
                    ORDER BY m.ModuleOrder, s.ScenarioOrder, s.ScenarioID;", conn))
                {
                    cmd.Parameters.Add("@LearnerID", SqlDbType.Int).Value = learnerId;
                    cmd.Parameters.Add("@Difficulty", SqlDbType.Int).Value = filterDifficulty ? (object)difficulty : DBNull.Value;
                    cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 110).Value =
                        string.IsNullOrEmpty(search) ? (object)DBNull.Value : "%" + search + "%";
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string difficultyLabel = UiHelper.DifficultyLabel(Convert.ToInt32(reader["DifficultyLevel"]));
                            string learnerStatus = Convert.ToString(reader["LearnerStatus"]);
                            bool completed = learnerStatus == "Completed" || learnerStatus == "Certified";
                            bool attempted = !completed && learnerStatus.Length > 0;
                            string text = Convert.ToString(reader["ScenarioText"]);

                            scenarios.Add(new ScenarioItem
                            {
                                ScenarioID = Convert.ToInt32(reader["ScenarioID"]),
                                Title = Convert.ToString(reader["Title"]),
                                Description = Convert.ToString(reader["Description"]),
                                Module = Convert.ToString(reader["ModuleTitle"]),
                                Difficulty = difficultyLabel,
                                DifficultyClass = difficultyLabel.ToLowerInvariant(),
                                // Reading time plus a couple of minutes to decide.
                                TimeEstimate = Math.Max(3, text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length / 200 + 2),
                                Status = completed ? "Completed" : attempted ? "Try again" : "Not started",
                                StatusBadge = completed ? "badge-success" : attempted ? "badge-warning" : "badge-secondary"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Load learner scenarios", ex.Message, ex.StackTrace);
                UiHelper.Notify(this, "Scenarios could not be loaded right now. Please try again.", "error");
            }

            rptScenarios.DataSource = scenarios;
            rptScenarios.DataBind();
            pnlNoScenarios.Visible = scenarios.Count == 0;
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
