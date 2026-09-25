<%@ Page Title="Grade Assignments - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GradeAssignments.aspx.cs" Inherits="RespondX.Expert.GradeAssignments" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Expert.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="upGrades" runat="server">
        <ContentTemplate>
            <div class="container crud-region">
                <div class="page-header">
                    <h1><i class="fas fa-check-double"></i> Grade Assignments</h1>
                    <p>Review and grade submissions from learners</p>
                </div>

                <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
                    <asp:Label ID="lblSuccess" runat="server"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </asp:Panel>

                <div class="modules-grid">
                    <asp:Repeater ID="rptSubmissions" runat="server" OnItemCommand="rptSubmissions_ItemCommand">
                        <ItemTemplate>
                            <div class="module-card">
                                <div class="module-header">
                                    <h3><%# Eval("AssignmentTitle") %></h3>
                                    <span class="badge <%# Eval("Status").ToString() == "Graded" ? "badge-success" : "badge-warning" %>">
                                        <%# Eval("Status") %>
                                    </span>
                                </div>
                                <p><strong>Learner:</strong> <%# Eval("LearnerName") %></p>
                                <p><strong>Submitted:</strong> <%# Eval("SubmittedAt", "{0:MMM dd, yyyy HH:mm}") %></p>
                                <div class="module-actions">
                                    <asp:Button ID="btnGrade" runat="server" Text='<%# Eval("Status").ToString() == "Graded" ? "Edit Grade" : "Grade" %>' CssClass="btn btn-info btn-sm" CommandName="Grade" CommandArgument='<%# Eval("SubmissionID") %>' />
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:Label ID="lblNoSubmissions" runat="server" Text="No submissions found" Visible='<%# rptSubmissions.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <div class="modal fade" id="modalGrade" tabindex="-1">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Grade Submission</h5>
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                            </div>
                            <div class="modal-body">
                                <asp:HiddenField ID="hfSubmissionID" runat="server" />
                                <asp:HiddenField ID="hfMaxScore" runat="server" />
                                <asp:Panel ID="pnlModalError" runat="server" CssClass="alert alert-danger" Visible="false">
                                    <asp:Label ID="lblModalError" runat="server"></asp:Label>
                                </asp:Panel>
                        
                                <div class="form-group">
                                    <label><strong>Content:</strong></label>
                                    <div class="p-3 bg-light border rounded">
                                        <asp:Literal ID="litContent" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="form-group" id="divFile" runat="server" visible="false">
                                    <label><strong>Attachment:</strong></label>
                                    <asp:HyperLink ID="hlFile" runat="server" Target="_blank" CssClass="btn btn-sm btn-outline-primary"><i class="fas fa-download"></i> Download File</asp:HyperLink>
                                </div>
                                <div class="form-row mt-4">
                                    <div class="form-group col-12">
                                        <label for="<%= txtScore.ClientID %>">Score <asp:Label ID="lblMaxScore" runat="server" CssClass="text-muted" /></label>
                                        <asp:TextBox ID="txtScore" runat="server" CssClass="form-control" TextMode="Number" min="0" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label>Feedback</label>
                                    <asp:TextBox ID="txtFeedback" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                <asp:Button ID="btnSaveGrade" runat="server" Text="Save Grade" CssClass="btn btn-primary" OnClick="btnSaveGrade_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
    
        </ContentTemplate>
    </asp:UpdatePanel>

    <style>
        .page-header { margin-bottom: 30px; }
        .page-header h1 { margin-bottom: 5px; }
        .page-header p { color: #718096; }
        .modules-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); gap: 20px; }
        .module-card { background: white; border-radius: 10px; padding: 20px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-top: 4px solid #667eea; }
        .module-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 10px; }
        .module-header h3 { margin: 0; font-size: 18px; color: #2d3748; }
        .module-actions { display: flex; gap: 8px; flex-wrap: wrap; margin-top: 15px; }
        .bg-light { background-color: #f8f9fa; }
        .border { border: 1px solid #dee2e6; }
        .rounded { border-radius: 0.25rem; }
        .p-3 { padding: 1rem; }
        .mt-4 { margin-top: 1.5rem; }
    </style>
</asp:Content>
