namespace RespondX.Learner
{
    public partial class Assignments
    {
        protected global::System.Web.UI.WebControls.Panel pnlSuccess;
        protected global::System.Web.UI.WebControls.Label lblSuccess;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Label lblError;
        protected global::System.Web.UI.WebControls.DropDownList ddlFilter;
        protected global::System.Web.UI.WebControls.Repeater rptAssignments;
        protected global::System.Web.UI.WebControls.HiddenField hfAssignmentID;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl lblModalAssignmentTitle;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl lblModalAssignmentDesc;
        protected global::System.Web.UI.WebControls.TextBox txtContent;
        protected global::System.Web.UI.WebControls.TextBox txtFileUrl;
        protected global::System.Web.UI.WebControls.Button btnSaveSubmission;

        public new RespondX.SiteMaster Master
        {
            get { return ((RespondX.SiteMaster)(base.Master)); }
        }
    }
}
