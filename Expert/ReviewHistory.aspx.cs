using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Expert
{
    public partial class ReviewHistory : Page
    {
        private int currentPage = 1;
        private int pageSize = 15;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Expert"))
                return;

            if (!IsPostBack)
            {
                LoadReviewHistory();
            }
        }

        private void LoadReviewHistory()
        {
            var history = GetReviewHistory();

            var status = ddlStatus.SelectedValue;
            if (status != "All")
            {
                history = history.FindAll(h => h.Status == status);
            }

            var contentType = ddlContentType.SelectedValue;
            if (contentType != "All")
            {
                history = history.FindAll(h => h.ContentType == contentType);
            }

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                history = history.FindAll(h =>
                    h.Title.ToLower().Contains(search.ToLower())
                );
            }

            int totalItems = history.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;

            var pagedHistory = new List<HistoryItem>();
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, totalItems);

            for (int i = startIndex; i < endIndex; i++)
            {
                pagedHistory.Add(history[i]);
            }

            rptHistory.DataSource = pagedHistory;
            rptHistory.DataBind();

            lblPageInfo.Text = $"Page {currentPage} of {(totalPages > 0 ? totalPages : 1)}";
            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < totalPages;
        }

        private List<HistoryItem> GetReviewHistory()
        {
            return new List<HistoryItem>
            {
                new HistoryItem { ReviewID = 1, Title = "Emergency Response Module", ContentType = "Module", Status = "Approved", Rating = 5, ReviewedAt = "2024-01-15" },
                new HistoryItem { ReviewID = 2, Title = "CPR Certification Quiz", ContentType = "Quiz", Status = "Needs Revision", Rating = 3, ReviewedAt = "2024-01-12" },
                new HistoryItem { ReviewID = 3, Title = "First Aid Basics Lesson", ContentType = "Lesson", Status = "Approved", Rating = 4, ReviewedAt = "2024-01-10" },
                new HistoryItem { ReviewID = 4, Title = "Fire Safety Scenario", ContentType = "Scenario", Status = "Rejected", Rating = 2, ReviewedAt = "2024-01-08" }
            };
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadReviewHistory();
        }

        protected void ddlContentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadReviewHistory();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadReviewHistory();
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                int reviewId = int.Parse(btn.CommandArgument);
                Response.Redirect($"ReviewContent.aspx?id={reviewId}");
            }
        }

        protected void btnReReview_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                int reviewId = int.Parse(btn.CommandArgument);
                Response.Redirect($"ReviewContent.aspx?id={reviewId}");
            }
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            currentPage--;
            LoadReviewHistory();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            currentPage++;
            LoadReviewHistory();
        }

        /// <summary>Renders a 0-5 rating as star images.</summary>
        protected string RenderStars(object rating)
        {
            int value;
            int.TryParse(Convert.ToString(rating), out value);
            value = Math.Max(0, Math.Min(5, value));

            string full = ResolveUrl("~/Content/Images/icons/star.svg");
            string empty = ResolveUrl("~/Content/Images/icons/star-empty.svg");
            var html = new System.Text.StringBuilder("<span class=\"star-rating\" title=\"" + value + " out of 5\">");
            for (int i = 1; i <= 5; i++)
            {
                html.Append("<img class=\"img-icon\" alt=\"\" src=\"").Append(i <= value ? full : empty).Append("\" />");
            }
            return html.Append("<span class=\"sr-only\">").Append(value).Append(" out of 5</span></span>").ToString();
        }
    }
}