<%@ Page Title="Manage Assignments - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageAssignments.aspx.cs" Inherits="RespondX.Expert.ManageAssignments" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Expert.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="upAssignments" runat="server">
        <ContentTemplate>
            <div class="container crud-region">
                <div class="page-header">
                    <h1><i class="fas fa-tasks"></i> Manage Assignments</h1>
                    <p>Create and edit assignments for your modules</p>
                </div>

                <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
                    <asp:Label ID="lblSuccess" runat="server"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </asp:Panel>

                <div class="filter-bar">
                    <div class="filter-group">
                        <asp:Button ID="btnAddAssignment" runat="server" Text="+ Add Assignment" CssClass="btn btn-success" OnClick="btnAddAssignment_Click" />
                    </div>
                </div>

                <div class="modules-grid">
                    <asp:Repeater ID="rptAssignments" runat="server" OnItemCommand="rptAssignments_ItemCommand">
                        <ItemTemplate>
                            <div class="module-card">
                                <div class="module-header">
                                    <h3><%# Eval("Title") %></h3>
                                    <span class="badge <%# Convert.ToBoolean(Eval("IsActive")) ? "badge-success" : "badge-secondary" %>">
                                        <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>
                                    </span>
                                </div>
                                <p><strong>Module:</strong> <%# Eval("ModuleTitle") %></p>
                                <p class="module-description"><%# Eval("Description") %></p>
                                <div class="module-meta">
                                    <span><i class="fas fa-calendar"></i> Due: <%# Eval("DueDate", "{0:MMM dd, yyyy}") %></span>
                                    <span><i class="fas fa-star"></i> Max: <%# Eval("MaxScore") %> pts</span>
                                </div>
                                <div class="module-actions">
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="Edit" CommandArgument='<%# Eval("AssignmentID") %>' />
                                    <asp:Button ID="btnToggle" runat="server" Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate" : "Activate" %>' CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "btn btn-warning btn-sm" : "btn btn-success btn-sm" %>' CommandName="Toggle" CommandArgument='<%# Eval("AssignmentID") %>' />
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:Label ID="lblNoAssignments" runat="server" Text="No assignments found" Visible='<%# rptAssignments.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <div class="modal fade" id="modalAssignment" tabindex="-1">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Add Assignment"></asp:Label></h5>
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                            </div>
                            <div class="modal-body">
                                <asp:HiddenField ID="hfAssignmentID" runat="server" />
                                <asp:Panel ID="pnlModalError" runat="server" CssClass="alert alert-danger" Visible="false">
                                    <asp:Label ID="lblModalError" runat="server"></asp:Label>
                                </asp:Panel>
                                <div class="form-group">
                                    <label>Module *</label>
                                    <asp:DropDownList ID="ddlModule" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label>Title *</label>
                                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="200" />
                                </div>
                                <div class="form-group">
                                    <label>Description</label>
                                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                                </div>
                                <div class="form-row">
                                    <div class="form-group col-6">
                                        <label>Due Date</label>
                                        <asp:TextBox ID="txtDueDate" runat="server" CssClass="form-control" TextMode="Date" />
                                    </div>
                                    <div class="form-group col-6">
                                        <label>Max Score</label>
                                        <asp:TextBox ID="txtMaxScore" runat="server" CssClass="form-control" TextMode="Number" min="1" max="1000" placeholder="100" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" Text="Active" CssClass="form-check-inline" />
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                <asp:Button ID="btnSaveAssignment" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveAssignment_Click" />
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
        .module-description { color: #718096; font-size: 14px; line-height: 1.6; margin-bottom: 12px; }
        .module-meta { display: flex; gap: 15px; font-size: 13px; color: #718096; margin-bottom: 12px; flex-wrap: wrap; }
        .module-actions { display: flex; gap: 8px; flex-wrap: wrap; }
        .filter-bar { display: flex; justify-content: space-between; flex-wrap: wrap; gap: 15px; margin-bottom: 20px; background: white; padding: 15px 20px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); }
        .filter-group { display: flex; gap: 10px; align-items: center; flex-wrap: wrap; }
    </style>
</asp:Content>
