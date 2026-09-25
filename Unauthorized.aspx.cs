using System;
using System.Web.UI;
using RespondX.Helpers;

namespace RespondX
{
    public partial class Unauthorized : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = 403;
            lnkDashboard.NavigateUrl = AuthorizationHelper.GetDashboardUrl();
        }
    }
}
