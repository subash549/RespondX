using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class ModuleDetails : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                int moduleId;
                if (int.TryParse(Request.QueryString["id"], out moduleId))
                {
                    LoadModuleDetails(moduleId);
                }
                else
                {
                    pnlModule.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadModuleDetails(int moduleId)
        {
            var module = GetModule(moduleId);
            if (module == null)
            {
                pnlModule.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            pnlModule.Visible = true;
            lblModuleTitle.Text = module.Title;
            lblModuleDescription.Text = module.Description;
            lblEstimatedHours.Text = module.EstimatedHours.ToString();
            lblLessonCount.Text = module.LessonCount.ToString();
            lblStatus.Text = module.Status;
            lblStatusBadge.Text = module.StatusBadge;
            lblProgressPercent.Text = module.ProgressPercentage.ToString();
            lblProgressDisplay.Text = module.ProgressPercentage + "%";
            lblCompletedLessons.Text = module.CompletedLessons.ToString();
            lblTotalLessons.Text = module.TotalLessons.ToString();

            LoadLessons(moduleId);
            LoadScenarios(moduleId);
            LoadQuiz(moduleId);
        }

        private ModuleItem GetModule(int moduleId)
        {
            var modules = new Dictionary<int, ModuleItem>
            {
                { 1, new ModuleItem { ModuleID = 1, Title = "Introduction to Emergency Response", Description = "Learn the fundamentals of emergency response and preparedness.", EstimatedHours = 2, Status = "In Progress", StatusBadge = "badge-warning", ProgressPercentage = 75, CompletedLessons = 4, TotalLessons = 5, LessonCount = 5 } },
                { 2, new ModuleItem { ModuleID = 2, Title = "CPR and First Aid", Description = "Comprehensive training in CPR and first aid techniques.", EstimatedHours = 4, Status = "Completed", StatusBadge = "badge-success", ProgressPercentage = 100, CompletedLessons = 8, TotalLessons = 8, LessonCount = 8 } }
            };

            return modules.ContainsKey(moduleId) ? modules[moduleId] : null;
        }

        private void LoadLessons(int moduleId)
        {
            var lessons = new List<LessonItem>
            {
                new LessonItem { LessonID = 1, Title = "What is Emergency Response?", Description = "Introduction to emergency response basics", IsCompleted = true },
                new LessonItem { LessonID = 2, Title = "Assessment of Emergency Situations", Description = "How to assess emergencies", IsCompleted = true },
                new LessonItem { LessonID = 3, Title = "Emergency Response Plans", Description = "Creating effective response plans", IsCompleted = false }
            };

            rptLessons.DataSource = lessons;
            rptLessons.DataBind();
        }

        private void LoadScenarios(int moduleId)
        {
            var scenarios = new List<ScenarioItem>
            {
                new ScenarioItem { ScenarioID = 1, Title = "Office Emergency", Description = "Handle an office emergency situation", Difficulty = "Intermediate" },
                new ScenarioItem { ScenarioID = 2, Title = "Home Emergency", Description = "Respond to a home emergency", Difficulty = "Beginner" }
            };

            rptScenarios.DataSource = scenarios;
            rptScenarios.DataBind();
        }

        private void LoadQuiz(int moduleId)
        {
            bool hasQuiz = moduleId == 1;
            if (hasQuiz)
            {
                pnlQuiz.Visible = true;
                pnlNoQuiz.Visible = false;
                lblQuizInfo.Text = "Test your knowledge with this module quiz. You need 70% to pass.";
                btnTakeQuiz.CommandArgument = moduleId.ToString();
            }
            else
            {
                pnlQuiz.Visible = false;
                pnlNoQuiz.Visible = true;
            }
        }

        protected void btnTakeQuiz_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                Response.Redirect($"~/Learner/TakeQuiz.aspx?moduleId={btn.CommandArgument}");
            }
        }
    }
}