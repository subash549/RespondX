<%@ Page Title="Assignments - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Assignments.aspx.cs" Inherits="RespondX.Learner.Assignments" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Learner.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-tasks"></i> My Assignments</h1>
            <p>Complete your module assignments to earn your certification</p>
        </div>

        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="filter-bar">
            <div class="filter-group">
                <label>Filter:</label>
                <asp:DropDownList ID="ddlFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged">
                    <asp:ListItem Value="All">All Assignments</asp:ListItem>
                    <asp:ListItem Value="Pending">Pending</asp:ListItem>
                    <asp:ListItem Value="Submitted">Submitted</asp:ListItem>
                    <asp:ListItem Value="Graded">Graded</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

        <div class="module-grid">
            <asp:Repeater ID="rptAssignments" runat="server" OnItemCommand="rptAssignments_ItemCommand">
                <ItemTemplate>
                    <div class="module-card">
                        <div class="module-header">
                            <h3 style="font-size: 18px; margin: 0;"><%# Eval("Title") %></h3>
                            <span class="badge <%# Eval("StatusBadge") %>"><%# Eval("Status") %></span>
                        </div>
                        <p class="text-muted mb-2"><%# Eval("ModuleTitle") %></p>
                        <div class="module-description"><%# Eval("Description") %></div>
                        <div class="module-meta">
                            <span><i class="fas fa-calendar"></i> Due: <%# Eval("DueDate", "{0:MMM dd, yyyy}") %></span>
                            <span><i class="fas fa-star"></i> Max: <%# Eval("MaxScore") %> pts</span>
                        </div>
                        <div class="mt-3">
                            <asp:Panel runat="server" Visible='<%# Eval("Status").ToString() == "Graded" %>'>
                                <div class="p-2 mb-2 bg-light rounded border">
                                    <strong>Score:</strong> <%# Eval("Score") %> / <%# Eval("MaxScore") %><br />
                                    <strong>Feedback:</strong> <%# Eval("Feedback") %>
                                </div>
                            </asp:Panel>
                            <asp:Button ID="btnSubmit" runat="server" Text='<%# Eval("Status").ToString() == "Pending" ? "Submit Assignment" : "View Submission" %>' 
                                CssClass='<%# Eval("Status").ToString() == "Pending" ? "btn btn-primary btn-sm" : "btn btn-info btn-sm" %>' 
                                CommandName="Submit" CommandArgument='<%# Eval("AssignmentID") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lblNoAssignments" runat="server" Text="No assignments found" Visible='<%# rptAssignments.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <div class="modal fade" id="modalSubmit" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Submit Assignment</h5>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hfAssignmentID" runat="server" />
                        <h4 id="lblModalAssignmentTitle" runat="server" class="mb-3"></h4>
                        <p id="lblModalAssignmentDesc" runat="server" class="text-muted"></p>
                        
                        <div class="form-group">
                            <label>Submission Content *</label>
                            <asp:TextBox ID="txtContent" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" />
                        </div>
                        <div class="form-group">
                            <label>Attachment URL (Optional)</label>
                            <asp:TextBox ID="txtFileUrl" runat="server" CssClass="form-control" placeholder="https://docs.google.com/..." />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="btnSaveSubmission" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSaveSubmission_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <style>
        .page-header { margin-bottom: 30px; }
        .page-header h1 { margin-bottom: 5px; }
        .page-header p { color: #718096; }
        .module-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); gap: 20px; }
        .module-card { background: white; border-radius: 10px; padding: 20px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-top: 4px solid #667eea; display: flex; flex-direction: column; }
        .module-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 10px; }
        .filter-bar { display: flex; justify-content: space-between; flex-wrap: wrap; gap: 15px; margin-bottom: 25px; background: white; padding: 15px 20px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); }
        .filter-group { display: flex; gap: 10px; align-items: center; }
        .module-description { flex: 1; margin-bottom: 15px; font-size: 14px; color: #4a5568; }
        .bg-light { background-color: #f8f9fa; }
        .rounded { border-radius: 0.25rem; }
        .border { border: 1px solid #dee2e6; }
        .p-2 { padding: 0.5rem; }
        .mb-2 { margin-bottom: 0.5rem; }
        .mt-3 { margin-top: 1rem; }
    </style>
</asp:Content>
