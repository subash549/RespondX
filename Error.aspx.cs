using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RespondX
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Guid errorReference;
            if (Guid.TryParseExact(Request.QueryString["ref"], "N", out errorReference))
            {
                pnlErrorReference.Visible = true;
                lblErrorReference.Text = errorReference.ToString("N");
            }
        }
    }
}
