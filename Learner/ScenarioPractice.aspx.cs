using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class ScenarioPractice : Page
    {
        /// <summary>Option as shown to the learner; feedback is only filled in after answering.</summary>
        public class OptionView
        {
            public int OptionID { get; set; }
            public string OptionText { get; set; }
            public string Feedback { get; set; }
            public string CssClass { get; set; }
            public string IconUrl { get; set; }
            public bool IsSelected { get; set; }
            public bool IsLocked { get; set; }
        }

        private sealed class ScenarioData
        {
            public int ScenarioID;
            public int ModuleID;
            public string Title;
            public string Description;
            public string ScenarioText;
            public int DifficultyLevel;
            public int? NextScenarioID;
            public List<ScenarioOptionItem> Options = new List<ScenarioOptionItem>();
        }

        private int ScenarioId
        {
            get
            {
                int id;
                return int.TryParse(hfScenarioId.Value, out id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                int scenarioId;
                var scenario = int.TryParse(Request.QueryString["id"], out scenarioId) ? GetScenario(scenarioId) : null;
                if (scenario == null || scenario.Options.Count == 0)
                {
                    pnlScenario.Visible = false;
                    pnlNotFound.Visible = true;
                    return;
                }

                pnlScenario.Visible = true;
                hfScenarioId.Value = scenario.ScenarioID.ToString();
                Page.Title = scenario.Title;
                lblTitle.Text = Server.HtmlEncode(scenario.Title);
                lblDifficulty.Text = UiHelper.DifficultyLabel(scenario.DifficultyLevel);
                lblDescription.Text = Server.HtmlEncode(scenario.Description);
                litScenarioText.Text = FormatText(scenario.ScenarioText);
                BindOptions(scenario, null);
            }
        }

        private static string FormatText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            // Scenario text is plain text typed by admins: encode it and keep paragraphs.
            var html = new System.Text.StringBuilder();
            foreach (var paragraph in Regex.Split(text.Trim(), @"\r?\n\s*\r?\n"))
            {
                html.Append("<p>").Append(HttpUtility.HtmlEncode(paragraph.Trim()).Replace("\n", "<br />")).Append("</p>");
            }
            return html.ToString();
        }

        private ScenarioData GetScenario(int scenarioId)
        {
            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                using (var cmd = new SqlCommand(@"
                    SELECT s.ScenarioID, s.ModuleID, s.Title, s.Description, s.ScenarioText, s.DifficultyLevel,
                           (SELECT TOP 1 n.ScenarioID FROM dbo.Scenarios n
                            WHERE n.ModuleID = s.ModuleID AND n.IsActive = 1 AND n.ScenarioID <> s.ScenarioID
                              AND (n.ScenarioOrder > s.ScenarioOrder OR (n.ScenarioOrder = s.ScenarioOrder AND n.ScenarioID > s.ScenarioID))
                              AND EXISTS (SELECT 1 FROM dbo.ScenarioOptions o WHERE o.ScenarioID = n.ScenarioID)
                            ORDER BY n.ScenarioOrder, n.ScenarioID) AS NextScenarioID
                    FROM dbo.Scenarios s
                    INNER JOIN dbo.Modules m ON m.ModuleID = s.ModuleID
                    WHERE s.ScenarioID = @ID AND s.IsActive = 1 AND m.IsActive = 1;

                    SELECT OptionID, OptionText, IsCorrect, Feedback, OptionOrder
                    FROM dbo.ScenarioOptions WHERE ScenarioID = @ID
                    ORDER BY OptionOrder, OptionID;", conn))
                {
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = scenarioId;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        var scenario = new ScenarioData
                        {
                            ScenarioID = Convert.ToInt32(reader["ScenarioID"]),
                            ModuleID = Convert.ToInt32(reader["ModuleID"]),
                            Title = Convert.ToString(reader["Title"]),
                            Description = Convert.ToString(reader["Description"]),
                            ScenarioText = Convert.ToString(reader["ScenarioText"]),
                            DifficultyLevel = Convert.ToInt32(reader["DifficultyLevel"]),
                            NextScenarioID = reader["NextScenarioID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["NextScenarioID"])
                        };

                        reader.NextResult();
                        while (reader.Read())
                        {
                            scenario.Options.Add(new ScenarioOptionItem
                            {
                                OptionID = Convert.ToInt32(reader["OptionID"]),
                                OptionText = Convert.ToString(reader["OptionText"]),
                                IsCorrect = Convert.ToBoolean(reader["IsCorrect"]),
                                Feedback = Convert.ToString(reader["Feedback"]),
                                OptionOrder = Convert.ToInt32(reader["OptionOrder"])
                            });
                        }
                        return scenario;
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Load scenario practice", ex.Message, ex.StackTrace);
                return null;
            }
        }

        /// <param name="selectedOptionId">Null before the learner answers.</param>
        private void BindOptions(ScenarioData scenario, int? selectedOptionId)
        {
            bool answered = selectedOptionId.HasValue;
            var views = new List<OptionView>();
            foreach (var option in scenario.Options)
            {
                bool isSelected = answered && option.OptionID == selectedOptionId.Value;
                var view = new OptionView
                {
                    OptionID = option.OptionID,
                    OptionText = option.OptionText,
                    IsSelected = isSelected,
                    IsLocked = answered,
                    CssClass = answered ? "is-locked" : string.Empty
                };

                // After answering, reveal the chosen option's feedback and the correct answer.
                if (answered && (isSelected || option.IsCorrect))
                {
                    view.CssClass += option.IsCorrect ? " correct" : " incorrect";
                    view.IconUrl = option.IsCorrect ? "~/Content/Images/icons/check.svg" : "~/Content/Images/icons/cross.svg";
                    view.Feedback = option.Feedback;
                }
                views.Add(view);
            }

            rptOptions.DataSource = views;
            rptOptions.DataBind();

            btnSubmitAnswer.Visible = !answered;
            pnlResult.Visible = answered;
            if (answered)
            {
                var chosen = scenario.Options.Find(o => o.OptionID == selectedOptionId.Value);
                bool correct = chosen != null && chosen.IsCorrect;
                pnlResult.CssClass = "scenario-result " + (correct ? "correct" : "incorrect");
                imgResult.Src = correct ? "~/Content/Images/icons/check.svg" : "~/Content/Images/icons/cross.svg";
                lblResult.Text = correct
                    ? "Well done! That's the best response."
                    : "Not quite. Review the feedback above and try again.";
                btnTryAgain.Visible = !correct;

                hlNextScenario.Visible = true;
                if (scenario.NextScenarioID.HasValue)
                {
                    hlNextScenario.NavigateUrl = "~/Learner/ScenarioPractice.aspx?id=" + scenario.NextScenarioID.Value;
                    litNextText.Text = "Next Scenario";
                }
                else
                {
                    hlNextScenario.NavigateUrl = "~/Learner/Scenarios.aspx";
                    litNextText.Text = "All Scenarios";
                }
            }
            else
            {
                btnTryAgain.Visible = false;
                hlNextScenario.Visible = false;
            }
        }

        protected void btnSubmitAnswer_Click(object sender, EventArgs e)
        {
            var scenario = GetScenario(ScenarioId);
            if (scenario == null)
            {
                UiHelper.Notify(this, "This scenario is no longer available.", "error");
                return;
            }

            int optionId;
            var chosen = int.TryParse(Request.Form["scenarioOption"], out optionId)
                ? scenario.Options.Find(o => o.OptionID == optionId)
                : null;
            if (chosen == null)
            {
                BindOptions(scenario, null);
                UiHelper.Notify(this, "Please choose an option first.", "warning");
                return;
            }

            RecordAttempt(scenario, chosen.IsCorrect);
            BindOptions(scenario, chosen.OptionID);
        }

        protected void btnTryAgain_Click(object sender, EventArgs e)
        {
            var scenario = GetScenario(ScenarioId);
            if (scenario != null)
                BindOptions(scenario, null);
        }

        // Stores the learner's result so the scenario list can show Completed / Try again.
        // A completed scenario stays completed even if it is practised again later.
        private void RecordAttempt(ScenarioData scenario, bool correct)
        {
            var learnerId = SessionHelper.GetCurrentUserId();
            if (!learnerId.HasValue)
                return;

            try
            {
                DatabaseHelper.EnsureLearnerProfile(learnerId.Value);
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                using (var cmd = new SqlCommand(@"
                    UPDATE dbo.LearnerProgress
                    SET Status = CASE WHEN Status = N'Completed' OR @Correct = 1 THEN N'Completed' ELSE N'InProgress' END,
                        ProgressPercentage = CASE WHEN Status = N'Completed' OR @Correct = 1 THEN 100 ELSE 50 END,
                        CompletedAt = CASE WHEN @Correct = 1 THEN ISNULL(CompletedAt, GETDATE()) ELSE CompletedAt END,
                        LastAccessedAt = GETDATE()
                    WHERE LearnerID = @LearnerID AND ScenarioID = @ScenarioID;

                    IF @@ROWCOUNT = 0
                        INSERT INTO dbo.LearnerProgress
                            (LearnerID, ModuleID, ScenarioID, Status, StartedAt, CompletedAt, LastAccessedAt, ProgressPercentage, TimeSpentMinutes)
                        VALUES
                            (@LearnerID, @ModuleID, @ScenarioID,
                             CASE WHEN @Correct = 1 THEN N'Completed' ELSE N'InProgress' END,
                             GETDATE(), CASE WHEN @Correct = 1 THEN GETDATE() END, GETDATE(),
                             CASE WHEN @Correct = 1 THEN 100 ELSE 50 END, 0);", conn))
                {
                    cmd.Parameters.Add("@LearnerID", SqlDbType.Int).Value = learnerId.Value;
                    cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = scenario.ModuleID;
                    cmd.Parameters.Add("@ScenarioID", SqlDbType.Int).Value = scenario.ScenarioID;
                    cmd.Parameters.Add("@Correct", SqlDbType.Bit).Value = correct;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // The learner still sees their result; only the history is affected.
                DatabaseHelper.LogError("Record scenario attempt", ex.Message, ex.StackTrace);
            }
        }
    }
}
