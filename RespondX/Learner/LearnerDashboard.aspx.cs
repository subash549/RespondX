using System;
using System.Collections.Generic;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class LearnerDashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            var userId = SessionHelper.GetCurrentUserId();
            if (!userId.HasValue) return;

            var learner = DatabaseHelper.GetLearnerById(userId.Value);
            if (learner != null)
            {
                lblUserName.Text = learner.FullName;
            }

            LoadStats(userId.Value);
            LoadRecentModules(userId.Value);
            LoadRecentActivity(userId.Value);
            LoadAlerts(userId.Value);
        }

        private void LoadStats(int learnerId)
        {
            lblTotalModules.Text = "8";
            lblCompletedModules.Text = "3";
            lblInProgressModules.Text = "2";
            lblCertificates.Text = "2";
        }

        private void LoadRecentModules(int learnerId)
        {
            var modules = new List<ModuleItem>
            {
                new ModuleItem { ModuleID = 1, Title = "Introduction to Emergency Response", Status = "In Progress", ProgressPercentage = 75 },
                new ModuleItem { ModuleID = 2, Title = "CPR and First Aid", Status = "Completed", ProgressPercentage = 100 },
                new ModuleItem { ModuleID = 3, Title = "Emergency Communication", Status = "Not Started", ProgressPercentage = 0 }
            };

            rptRecentModules.DataSource = modules;
            rptRecentModules.DataBind();
        }

        private void LoadRecentActivity(int learnerId)
        {
            var activities = new List<ActivityItem>
            {
                new ActivityItem { Title = "Completed Module 2", Description = "Finished CPR and First Aid module", ActivityType = "completed", Icon = "check-circle", TimeAgo = "2 hours ago" },
                new ActivityItem { Title = "Started Quiz", Description = "Took Emergency Response quiz", ActivityType = "quiz", Icon = "question-circle", TimeAgo = "5 hours ago" },
                new ActivityItem { Title = "Practiced Scenario", Description = "Completed emergency communication scenario", ActivityType = "scenario", Icon = "users", TimeAgo = "1 day ago" }
            };

            rptRecentActivity.DataSource = activities;
            rptRecentActivity.DataBind();
        }

        private void LoadAlerts(int learnerId)
        {
            var alerts = new List<AlertItem>
            {
                new AlertItem { Title = "New Module Available", Message = "Advanced Emergency Response has been added", AlertType = "notification", CreatedAt = DateTime.Now.AddHours(-3), TimeAgo = "3 hours ago" },
                new AlertItem { Title = "Reminder", Message = "Complete your CPR certification quiz", AlertType = "reminder", CreatedAt = DateTime.Now.AddDays(-1), TimeAgo = "1 day ago" }
            };

            rptAlerts.DataSource = alerts;
            rptAlerts.DataBind();
        }
    }
}