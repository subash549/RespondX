using System;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Expert
{
    public partial class ReviewContent : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Expert"))
                return;

            if (!IsPostBack)
            {
                int reviewId;
                if (int.TryParse(Request.QueryString["id"], out reviewId))
                {
                    LoadReview(reviewId);
                }
                else
                {
                    pnlReview.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadReview(int reviewId)
        {
            var review = GetReview(reviewId);
            if (review == null)
            {
                pnlReview.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            pnlReview.Visible = true;
            lblTitle.Text = review.Title;
            lblContentType.Text = review.ContentType;
            lblSubmittedBy.Text = review.SubmittedBy;
            lblPriority.Text = review.Priority;
            lblPriorityClass.Text = review.PriorityClass.ToLower();
            litContent.Text = review.Content;

            LoadDraft(reviewId);
        }

        private ReviewContentItem GetReview(int reviewId)
        {
            return new ReviewContentItem
            {
                ReviewID = 1,
                Title = "Advanced Emergency Response Module",
                ContentType = "Module",
                SubmittedBy = "Admin",
                Priority = "High",
                PriorityClass = "high",
                Content = @"
                    <h2>Module Overview</h2>
                    <p>This module covers advanced emergency response techniques for complex situations.</p>
                    <h3>Learning Objectives</h3>
                    <ul>
                        <li>Understand advanced triage protocols</li>
                        <li>Coordinate multi-agency emergency response</li>
                        <li>Implement mass casualty incident management</li>
                    </ul>
                "
            };
        }

        private void LoadDraft(int reviewId)
        {
            txtFeedback.Text = "";
            rblRating.ClearSelection();
            ddlRecommendation.SelectedIndex = 0;
        }

        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            if (ValidateReview())
            {
                ShowSuccess("Review submitted successfully!");
                Response.AddHeader("REFRESH", "2;URL=PendingReviews.aspx");
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            ShowSuccess("Draft saved successfully!");
        }

        private bool ValidateReview()
        {
            if (string.IsNullOrEmpty(txtFeedback.Text))
            {
                ShowError("Please provide feedback comments.");
                return false;
            }

            if (string.IsNullOrEmpty(rblRating.SelectedValue))
            {
                ShowError("Please select a rating.");
                return false;
            }

            return true;
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