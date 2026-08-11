using System;
using System.Collections.Generic;
using System.Web.UI;
using RespondX.Helpers;
using RespondX.Models;

namespace RespondX.Learner
{
    public partial class Modules : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationHelper.RequireRole("Learner"))
                return;

            if (!IsPostBack)
            {
                LoadModules();
            }
        }

        private void LoadModules()
        {
            var modules = GetModules();

            var filter = ddlFilter.SelectedValue;
            if (filter != "All")
            {
                modules = modules.FindAll(m => m.Status.Replace(" ", "") == filter);
            }

            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                modules = modules.FindAll(m =>
                    m.Title.ToLower().Contains(search.ToLower()) ||
                    m.Description.ToLower().Contains(search.ToLower())
                );
            }

            rptModules.DataSource = modules;
            rptModules.DataBind();
        }

        private List<ModuleItem> GetModules()
        {
            return new List<ModuleItem>
            {
                new ModuleItem
                {
                    ModuleID = 1,
                    Title = "Introduction to Emergency Response",
                    Description = "Learn the fundamentals of emergency response and preparedness.",
                    EstimatedHours = 2,
                    Status = "In Progress",
                    StatusBadge = "badge-warning",
                    ProgressPercentage = 75,
                    IsCompleted = false,
                    LessonCount = 5
                },
                new ModuleItem
                {
                    ModuleID = 2,
                    Title = "CPR and First Aid",
                    Description = "Comprehensive training in CPR and first aid techniques.",
                    EstimatedHours = 4,
                    Status = "Completed",
                    StatusBadge = "badge-success",
                    ProgressPercentage = 100,
                    IsCompleted = true,
                    LessonCount = 8
                },
                new ModuleItem
                {
                    ModuleID = 3,
                    Title = "Emergency Communication",
                    Description = "Effective communication strategies during emergencies.",
                    EstimatedHours = 3,
                    Status = "Not Started",
                    StatusBadge = "badge-secondary",
                    ProgressPercentage = 0,
                    IsCompleted = false,
                    LessonCount = 6
                }
            };
        }

        protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadModules();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadModules();
        }
    }
}