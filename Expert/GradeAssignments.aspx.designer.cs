namespace RespondX.Expert
{
    public partial class GradeAssignments
    {
        protected global::System.Web.UI.WebControls.Panel pnlSuccess;
        protected global::System.Web.UI.WebControls.Label lblSuccess;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Label lblError;
        protected global::System.Web.UI.WebControls.Repeater rptSubmissions;
        protected global::System.Web.UI.WebControls.HiddenField hfSubmissionID;
        protected global::System.Web.UI.WebControls.Literal litContent;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl divFile;
        protected global::System.Web.UI.WebControls.HyperLink hlFile;
        protected global::System.Web.UI.WebControls.TextBox txtScore;
        protected global::System.Web.UI.WebControls.TextBox txtFeedback;
        protected global::System.Web.UI.WebControls.Button btnSaveGrade;

        public new RespondX.SiteMaster Master
        {
            get { return ((RespondX.SiteMaster)(base.Master)); }
        }
    }
}
