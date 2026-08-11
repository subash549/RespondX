using System;
using System.Collections.Generic;
using System.Web.UI;
using RespondX.Helpers;

namespace RespondX.Learner
{
    public partial class MyProgress : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                LoadProgress();
            }
        }

        private void LoadProgress()
        {
            var userId = SessionHelper.GetCurrentUserId();
            if (!userId.HasValue) return;

            // In production, load from database
            int overallProgress = 65;
            int completedModules = 3;
            int totalModules = 8;
            int certificatesEarned = 2;
            int totalHours = 12;

            lblOverallProgress.Text = overallProgress.ToString();
            lblOverallProgressBar.Text = overallProgress.ToString();
            lblCompletedModules.Text = $"{completedModules}/{totalModules}";
            lblCertificatesEarned.Text = certificatesEarned.ToString();
            lblTotalHours.Text = totalHours.ToString();

            LoadModuleProgress();
            LoadAchievements();
            RegisterCharts();
        }

        private void LoadModuleProgress()
        {
            var modules = new List<ModuleProgress>
            {
                new ModuleProgress { ModuleTitle = "Introduction to Emergency Response", Status = "In Progress", StatusBadge = "badge-warning", ProgressPercentage = 75, TimeSpent = "2h 30m", LastActivity = "Today" },
                new ModuleProgress { ModuleTitle = "CPR and First Aid", Status = "Completed", StatusBadge = "badge-success", ProgressPercentage = 100, TimeSpent = "4h 15m", LastActivity = "Yesterday" },
                new ModuleProgress { ModuleTitle = "Emergency Communication", Status = "Not Started", StatusBadge = "badge-secondary", ProgressPercentage = 0, TimeSpent = "0h 0m", LastActivity = "N/A" },
                new ModuleProgress { ModuleTitle = "Fire Safety", Status = "In Progress", StatusBadge = "badge-warning", ProgressPercentage = 45, TimeSpent = "1h 45m", LastActivity = "2 days ago" }
            };

            rptModuleProgress.DataSource = modules;
            rptModuleProgress.DataBind();
        }

        private void LoadAchievements()
        {
            var achievements = new List<Achievement>
            {
                new Achievement { Title = "First Steps", Description = "Complete your first module", Icon = "rocket", IsEarned = true },
                new Achievement { Title = "Knowledge Seeker", Description = "Complete 3 modules", Icon = "graduation-cap", IsEarned = true },
                new Achievement { Title = "Emergency Expert", Description = "Complete all modules", Icon = "trophy", IsEarned = false },
                new Achievement { Title = "Perfect Score", Description = "Score 100% on any quiz", Icon = "star", IsEarned = false },
                new Achievement { Title = "Certificate Collector", Description = "Earn 5 certificates", Icon = "award", IsEarned = false }
            };

            rptAchievements.DataSource = achievements;
            rptAchievements.DataBind();
        }

        private void RegisterCharts()
        {
            string script = @"
                // Module Progress Chart
                var ctx1 = document.getElementById('moduleProgressChart').getContext('2d');
                new Chart(ctx1, {
                    type: 'bar',
                    data: {
                        labels: ['Emergency Response', 'CPR & First Aid', 'Communication', 'Fire Safety'],
                        datasets: [{
                            label: 'Progress (%)',
                            data: [75, 100, 0, 45],
                            backgroundColor: ['#667eea', '#48bb78', '#a0aec0', '#f6ad55'],
                            borderWidth: 0
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: true,
                        scales: {
                            y: { beginAtZero: true, max: 100 }
                        },
                        plugins: {
                            legend: { display: false }
                        }
                    }
                });

                // Performance Chart
                var ctx2 = document.getElementById('performanceChart').getContext('2d');
                new Chart(ctx2, {
                    type: 'doughnut',
                    data: {
                        labels: ['Exemplary', 'Proficient', 'Developing', 'Beginning'],
                        datasets: [{
                            data: [2, 1, 1, 0],
                            backgroundColor: ['#48bb78', '#667eea', '#f6ad55', '#fc8181'],
                            borderWidth: 0
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: true,
                        plugins: {
                            legend: { position: 'bottom' }
                        }
                    }
                });
            ";

            ClientScript.RegisterStartupScript(this.GetType(), "charts", script, true);
        }
    }

    public class ModuleProgress
    {
        public string ModuleTitle { get; set; }
        public string Status { get; set; }
        public string StatusBadge { get; set; }
        public int ProgressPercentage { get; set; }
        public string TimeSpent { get; set; }
        public string LastActivity { get; set; }
    }

    public class Achievement
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool IsEarned { get; set; }
    }
}