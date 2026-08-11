using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Expert
{
    public partial class PendingReviews : Page
    {
        private int currentPage = 1;
        private int pageSize = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Expert"))
                return;

            if (!IsPostBack)
            {
                LoadPendingReviews();
            }
        }

        private void LoadPendingReviews()
        {
            var reviews = GetPendingReviews();

            var contentType = ddlContentType.SelectedValue;
            if (contentType != "All")
            {
                reviews = reviews.FindAll(r => r.ContentType == contentType);
            }

            var priority = ddlPriority.SelectedValue;
            if (priority != "All")
            {
                reviews = reviews.FindAll(r => r.Priority == priority);
            }

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                reviews = reviews.FindAll(r =>
                    r.Title.ToLower().Contains(search.ToLower()) ||
                    (r.Description != null && r.Description.ToLower().Contains(search.ToLower()))
                );
            }

            int totalItems = reviews.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;

            var pagedReviews = new List<PendingReviewItem>();
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, totalItems);

            for (int i = startIndex; i < endIndex; i++)
            {
                pagedReviews.Add(reviews[i]);
            }

            rptPendingReviews.DataSource = pagedReviews;
            rptPendingReviews.DataBind();

            lblPageInfo.Text = $"Page {currentPage} of {(totalPages > 0 ? totalPages : 1)}";
            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < totalPages;
        }

        private List<PendingReviewItem> GetPendingReviews()
        {
            return new List<PendingReviewItem>
            {
                new PendingReviewItem
                {
                    ReviewID = 1,
                    Title = "Advanced Emergency Response Module",
                    ContentType = "Module",
                    SubmittedBy = "Admin",
                    AssignedAt = "2 days ago",
                    DueDate = "5 days from now",
                    Priority = "High",
                    PriorityClass = "high",
                    Description = "This module covers advanced emergency response techniques and protocols for complex situations."
                },
                new PendingReviewItem
                {
                    ReviewID = 2,
                    Title = "Fire Safety Quiz",
                    ContentType = "Quiz",
                    SubmittedBy = "Admin",
                    AssignedAt = "3 days ago",
                    DueDate = "7 days from now",
                    Priority = "Medium",
                    PriorityClass = "medium",
                    Description = "Comprehensive quiz on fire safety procedures and emergency evacuation protocols."
                },
                new PendingReviewItem
                {
                    ReviewID = 3,
                    Title = "Communication in Emergencies",
                    ContentType = "Lesson",
                    SubmittedBy = "Admin",
                    AssignedAt = "1 week ago",
                    DueDate = "3 days from now",
                    Priority = "Low",
                    PriorityClass = "low",
                    Description = "Lesson on effective communication strategies during emergency situations."
                }
            };
        }

        protected void ddlContentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadPendingReviews();
        }

        protected void ddlPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadPendingReviews();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadPendingReviews();
        }

        protected void btnSkip_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                ShowSuccess("Review skipped. It will be reassigned to another expert.");
                LoadPendingReviews();
            }
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            currentPage--;
            LoadPendingReviews();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            currentPage++;
            LoadPendingReviews();
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            lblSuccess.Text = message;
            pnlError.Visible = false;
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
            pnlSuccess.Visible = false;
        }
    }
}