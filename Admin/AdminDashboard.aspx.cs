using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT (SELECT COUNT(*) FROM dbo.Users),
                       (SELECT COUNT(*) FROM dbo.Modules),
                       (SELECT COUNT(*) FROM dbo.Quizzes),
                       (SELECT COUNT(*) FROM dbo.Certificates);", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblTotalUsers.Text = Convert.ToInt32(reader[0]).ToString();
                        lblTotalModules.Text = Convert.ToInt32(reader[1]).ToString();
                        lblTotalQuizzes.Text = Convert.ToInt32(reader[2]).ToString();
                        lblTotalCertificates.Text = Convert.ToInt32(reader[3]).ToString();
                    }
                }
            }

            LoadRecentUsers();
            LoadRecentActivity();
            LoadAlerts();
        }

        private void LoadRecentUsers()
        {
            var users = new List<UserItem>();
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"SELECT TOP (5) UserID, Username, Email, FirstName, LastName, Role, IsActive, CreatedAt
                FROM dbo.Users ORDER BY CreatedAt DESC, UserID DESC;", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string first = Convert.ToString(reader["FirstName"]);
                        string last = Convert.ToString(reader["LastName"]);
                        users.Add(new UserItem
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            Username = Convert.ToString(reader["Username"]),
                            Email = Convert.ToString(reader["Email"]),
                            FirstName = first,
                            LastName = last,
                            FullName = (first + " " + last).Trim(),
                            Role = Convert.ToString(reader["Role"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }

            rptRecentUsers.DataSource = users;
            rptRecentUsers.DataBind();
        }

        private void LoadRecentActivity()
        {
            var activities = new List<ActivityItem>();
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT TOP (8) ActivityTitle AS Title, ActivityDescription AS Description, ActivityAt
                FROM (
                    SELECT N'User Registration' AS ActivityTitle,
                           FirstName + N' ' + LastName + N' registered as ' + LOWER(Role) AS ActivityDescription,
                           CreatedAt AS ActivityAt, UserID AS SortID
                    FROM dbo.Users
                    UNION ALL
                    SELECT N'Module Created', N'Module added: ' + Title, CreatedAt, ModuleID FROM dbo.Modules
                    UNION ALL
                    SELECT N'Quiz Created', N'Quiz added: ' + Title, CreatedAt, QuizID FROM dbo.Quizzes
                    UNION ALL
                    SELECT N'Lesson Created', N'Lesson added: ' + Title, CreatedAt, LessonID FROM dbo.Lessons
                    UNION ALL
                    SELECT N'Scenario Created', N'Scenario added: ' + Title, CreatedAt, ScenarioID FROM dbo.Scenarios
                ) activity
                ORDER BY ActivityAt DESC, SortID DESC;", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime when = Convert.ToDateTime(reader["ActivityAt"]);
                        activities.Add(new ActivityItem
                        {
                            Title = Convert.ToString(reader["Title"]),
                            Description = Convert.ToString(reader["Description"]),
                            TimeAgo = RespondX.Helpers.UiHelper.TimeAgo(when)
                        });
                    }
                }
            }

            rptRecentActivity.DataSource = activities;
            rptRecentActivity.DataBind();
        }

        private void LoadAlerts()
        {
            rptAlerts.DataSource = AlertRepository.GetAlertGroups(null, "Active");
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
