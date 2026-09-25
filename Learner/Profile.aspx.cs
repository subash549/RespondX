using System;
using System.Web.UI;

namespace RespondX.Learner
{
    public partial class Profile : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Profile.aspx"), false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
