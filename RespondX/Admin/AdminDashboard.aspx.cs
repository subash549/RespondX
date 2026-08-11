using System;
using System.Collections.Generic;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class AdminDashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                LoadDashboardData();
                RegisterCharts();
            }
        }

        private void LoadDashboardData()
        {
            var user = SessionHelper.GetCurrentUser();
            if (user != null)
            {
                lblAdminName.Text = user.FullName;
            }

            lblTotalUsers.Text = "156";
            lblTotalModules.Text = "12";
            lblTotalQuizzes.Text = "24";
            lblTotalCertificates.Text = "67";

            LoadRecentUsers();
            LoadRecentActivity();
            LoadAlerts();
        }

        private void LoadRecentUsers()
        {
            var users = new List<UserItem>
            {
                new UserItem { FullName = "John Smith", Email = "john@email.com", IsActive = true, Role = "Learner" },
                new UserItem { FullName = "Mary Johnson", Email = "mary@email.com", IsActive = true, Role = "Expert" },
                new UserItem { FullName = "Robert Wilson", Email = "robert@email.com", IsActive = false, Role = "Learner" }
            };

            rptRecentUsers.DataSource = users;
            rptRecentUsers.DataBind();
        }

        private void LoadRecentActivity()
        {
            var activities = new List<ActivityItem>
            {
                new ActivityItem { Title = "User Registration", Description = "Jane Doe registered as a learner", TimeAgo = "2 hours ago" },
                new ActivityItem { Title = "Module Created", Description = "Advanced First Aid module was created", TimeAgo = "5 hours ago" },
                new ActivityItem { Title = "Quiz Completed", Description = "John Smith completed CPR quiz with 92%", TimeAgo = "1 day ago" }
            };

            rptRecentActivity.DataSource = activities;
            rptRecentActivity.DataBind();
        }

        private void LoadAlerts()
        {
            var alerts = new List<AlertItem>
            {
                new AlertItem { Title = "System Maintenance", Message = "Scheduled maintenance this weekend", PriorityClass = "medium", TimeAgo = "3 hours ago" },
                new AlertItem { Title = "User Report", Message = "3 users have been inactive for over 30 days", PriorityClass = "high", TimeAgo = "1 day ago" }
            };

            rptAlerts.DataSource = alerts;
            rptAlerts.DataBind();
        }

        private void RegisterCharts()
        {
            string script = @"
                var ctx1 = document.getElementById('userGrowthChart').getContext('2d');
                new Chart(ctx1, {
                    type: 'line',
                    data: {
                        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                        datasets: [{
                            label: 'New Users',
                            data: [12, 19, 15, 22, 28, 34],
                            borderColor: '#667eea',
                            backgroundColor: 'rgba(102, 126, 234, 0.1)',
                            borderWidth: 2,
                            fill: true
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: true,
                        plugins: { legend: { display: false } },
                        scales: { y: { beginAtZero: true } }
                    }
                });

                var ctx2 = document.getElementById('moduleCompletionChart').getContext('2d');
                new Chart(ctx2, {
                    type: 'bar',
                    data: {
                        labels: ['Module 1', 'Module 2', 'Module 3', 'Module 4', 'Module 5'],
                        datasets: [{
                            label: 'Completion Rate',
                            data: [85, 92, 78, 95, 60],
                            backgroundColor: ['#667eea', '#48bb78', '#f6ad55', '#fc8181', '#9f7aea'],
                            borderWidth: 0
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: true,
                        plugins: { legend: { display: false } },
                        scales: { y: { beginAtZero: true, max: 100 } }
                    }
                });
            ";

            ClientScript.RegisterStartupScript(this.GetType(), "charts", script, true);
        }
    }
}