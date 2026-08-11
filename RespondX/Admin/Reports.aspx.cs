using System;
using System.Collections.Generic;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Admin
{
    public partial class Reports : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Admin"))
                return;

            if (!IsPostBack)
            {
                LoadReport();
                RegisterCharts();
            }
        }

        private void LoadReport()
        {
            string reportType = ddlReportType.SelectedValue;
            int period = int.Parse(ddlPeriod.SelectedValue);

            pnlOverview.Visible = false;
            pnlUsers.Visible = false;
            pnlModules.Visible = false;
            pnlQuizzes.Visible = false;
            pnlCertificates.Visible = false;

            switch (reportType)
            {
                case "Overview":
                    pnlOverview.Visible = true;
                    LoadOverview(period);
                    break;
                case "Users":
                    pnlUsers.Visible = true;
                    LoadUserReport(period);
                    break;
                case "Modules":
                    pnlModules.Visible = true;
                    LoadModuleReport(period);
                    break;
                case "Quizzes":
                    pnlQuizzes.Visible = true;
                    LoadQuizReport(period);
                    break;
                case "Certificates":
                    pnlCertificates.Visible = true;
                    LoadCertificateReport(period);
                    break;
            }
        }

        private void LoadOverview(int period)
        {
            lblTotalUsers.Text = "156";
            lblActiveLearners.Text = "89";
            lblCompletionRate.Text = "67%";
            lblCertificatesIssued.Text = "45";

            lblUserChange.Text = "+12% this period";
            lblUserChange.CssClass = "stat-change positive";

            lblLearnerChange.Text = "+8% this period";
            lblLearnerChange.CssClass = "stat-change positive";

            lblCompletionChange.Text = "+5% this period";
            lblCompletionChange.CssClass = "stat-change positive";

            lblCertChange.Text = "+3% this period";
            lblCertChange.CssClass = "stat-change positive";
        }

        private void LoadUserReport(int period)
        {
            var users = new List<UserReportItem>
            {
                new UserReportItem { Role = "Learners", Total = 120, Active = 89, Inactive = 31, New = 15 },
                new UserReportItem { Role = "Experts", Total = 25, Active = 20, Inactive = 5, New = 3 },
                new UserReportItem { Role = "Admins", Total = 11, Active = 11, Inactive = 0, New = 0 }
            };

            rptUserReport.DataSource = users;
            rptUserReport.DataBind();
        }

        private void LoadModuleReport(int period)
        {
            var modules = new List<ModuleReportItem>
            {
                new ModuleReportItem { ModuleName = "Emergency Response", Enrollments = 85, CompletionRate = 75, AvgScore = 82, AvgTime = "2h 15m" },
                new ModuleReportItem { ModuleName = "CPR and First Aid", Enrollments = 72, CompletionRate = 82, AvgScore = 88, AvgTime = "3h 30m" },
                new ModuleReportItem { ModuleName = "Emergency Communication", Enrollments = 60, CompletionRate = 60, AvgScore = 75, AvgTime = "2h 45m" }
            };

            rptModuleReport.DataSource = modules;
            rptModuleReport.DataBind();
        }

        private void LoadQuizReport(int period)
        {
            var quizzes = new List<QuizReportItem>
            {
                new QuizReportItem { QuizName = "Emergency Response Quiz", Attempts = 120, PassRate = 78, AvgScore = 75, AvgTime = "18m" },
                new QuizReportItem { QuizName = "CPR Certification", Attempts = 85, PassRate = 85, AvgScore = 82, AvgTime = "25m" },
                new QuizReportItem { QuizName = "Communication Skills", Attempts = 65, PassRate = 65, AvgScore = 70, AvgTime = "15m" }
            };

            rptQuizReport.DataSource = quizzes;
            rptQuizReport.DataBind();
        }

        private void LoadCertificateReport(int period)
        {
            var certificates = new List<CertificateReportItem>
            {
                new CertificateReportItem { Month = "January", Issued = 12, Valid = 10, Expired = 2, Revoked = 0 },
                new CertificateReportItem { Month = "February", Issued = 8, Valid = 7, Expired = 1, Revoked = 0 },
                new CertificateReportItem { Month = "March", Issued = 15, Valid = 14, Expired = 1, Revoked = 0 },
                new CertificateReportItem { Month = "April", Issued = 10, Valid = 9, Expired = 1, Revoked = 0 }
            };

            rptCertificateReport.DataSource = certificates;
            rptCertificateReport.DataBind();
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
                            label: 'Users',
                            data: [100, 110, 125, 130, 145, 156],
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
                            data: [75, 82, 60, 90, 65],
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

                var ctx3 = document.getElementById('quizPerformanceChart').getContext('2d');
                new Chart(ctx3, {
                    type: 'bar',
                    data: {
                        labels: ['Quiz 1', 'Quiz 2', 'Quiz 3', 'Quiz 4'],
                        datasets: [{
                            label: 'Avg. Score',
                            data: [75, 82, 70, 88],
                            backgroundColor: 'rgba(102, 126, 234, 0.6)',
                            borderColor: '#667eea',
                            borderWidth: 1
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: true,
                        plugins: { legend: { display: false } },
                        scales: { y: { beginAtZero: true, max: 100 } }
                    }
                });

                var ctx4 = document.getElementById('engagementChart').getContext('2d');
                new Chart(ctx4, {
                    type: 'doughnut',
                    data: {
                        labels: ['Active', 'Inactive', 'New'],
                        datasets: [{
                            data: [89, 31, 15],
                            backgroundColor: ['#48bb78', '#a0aec0', '#667eea'],
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

        protected void ddlReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReport();
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReport();
        }

        protected void btnExportPDF_Click(object sender, EventArgs e)
        {
            ShowNotification("PDF report generated successfully", "success");
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            ShowNotification("Excel report generated successfully", "success");
        }

        private void ShowNotification(string message, string type)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "notify",
                $"showNotification('{message}', '{type}');", true);
        }
    }
}