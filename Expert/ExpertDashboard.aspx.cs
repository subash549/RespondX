using System;
using System.Collections.Generic;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Expert
{
    public partial class ExpertDashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Expert"))
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
                lblExpertName.Text = user.FullName;
            }

            lblPendingReviews.Text = "5";
            lblTotalReviewed.Text = "34";
            lblTotalContent.Text = "67";
            lblOverdueReviews.Text = "2";
            lblPendingBadge.Text = "5";

            // Temporary mock data for assignments (since there isn't a complex query set up yet)
            lblTotalAssignments.Text = "12";
            lblPendingGrades.Text = "8";
            lblPendingGradesBadge.Text = "8";

            LoadRecentReviews();
            LoadPendingReviews();
        }

        private void LoadRecentReviews()
        {
            var reviews = new List<ReviewItem>
            {
                new ReviewItem { Title = "CPR Module Review", ContentType = "Module", Status = "Approved", StatusBadge = "badge-success", TimeAgo = "2 hours ago" },
                new ReviewItem { Title = "Emergency Quiz", ContentType = "Quiz", Status = "Needs Revision", StatusBadge = "badge-warning", TimeAgo = "5 hours ago" },
                new ReviewItem { Title = "First Aid Lesson", ContentType = "Lesson", Status = "Approved", StatusBadge = "badge-success", TimeAgo = "1 day ago" }
            };

            rptRecentReviews.DataSource = reviews;
            rptRecentReviews.DataBind();
        }

        private void LoadPendingReviews()
        {
            var pending = new List<PendingReviewItem>
            {
                new PendingReviewItem { ReviewID = 1, Title = "Advanced Emergency Response Module", ContentType = "Module", Priority = "High", PriorityClass = "high", AssignedAt = "2 days ago" },
                new PendingReviewItem { ReviewID = 2, Title = "Fire Safety Quiz", ContentType = "Quiz", Priority = "Medium", PriorityClass = "medium", AssignedAt = "3 days ago" },
                new PendingReviewItem { ReviewID = 3, Title = "Communication Lesson", ContentType = "Lesson", Priority = "Low", PriorityClass = "low", AssignedAt = "1 week ago" }
            };

            rptPendingReviews.DataSource = pending;
            rptPendingReviews.DataBind();
        }

        private void RegisterCharts()
        {
            string script = @"
                var ctx1 = document.getElementById('reviewActivityChart').getContext('2d');
                new Chart(ctx1, {
                    type: 'line',
                    data: {
                        labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
                        datasets: [{
                            label: 'Reviews Completed',
                            data: [2, 3, 1, 4, 2, 0, 1],
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

                var ctx2 = document.getElementById('reviewStatusChart').getContext('2d');
                new Chart(ctx2, {
                    type: 'doughnut',
                    data: {
                        labels: ['Approved', 'Needs Revision', 'Pending', 'Rejected'],
                        datasets: [{
                            data: [45, 25, 20, 10],
                            backgroundColor: ['#48bb78', '#f6ad55', '#667eea', '#fc8181'],
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

    public class ReviewItem
    {
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string Status { get; set; }
        public string StatusBadge { get; set; }
        public string TimeAgo { get; set; }
    }
}