namespace RespondX.Expert
{
    public partial class ManageAssignments
    {
        protected global::System.Web.UI.WebControls.Panel pnlSuccess;
        protected global::System.Web.UI.WebControls.Label lblSuccess;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Label lblError;
        protected global::System.Web.UI.WebControls.Button btnAddAssignment;
        protected global::System.Web.UI.WebControls.Repeater rptAssignments;
        protected global::System.Web.UI.WebControls.Label lblModalTitle;
        protected global::System.Web.UI.WebControls.HiddenField hfAssignmentID;
        protected global::System.Web.UI.WebControls.DropDownList ddlModule;
        protected global::System.Web.UI.WebControls.TextBox txtTitle;
        protected global::System.Web.UI.WebControls.TextBox txtDescription;
        protected global::System.Web.UI.WebControls.TextBox txtDueDate;
        protected global::System.Web.UI.WebControls.TextBox txtMaxScore;
        protected global::System.Web.UI.WebControls.CheckBox chkIsActive;
        protected global::System.Web.UI.WebControls.Button btnSaveAssignment;

        public new RespondX.SiteMaster Master
        {
            get { return ((RespondX.SiteMaster)(base.Master)); }
        }
    }
}
