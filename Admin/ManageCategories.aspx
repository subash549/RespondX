<%@ Page Title="Manage Categories - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageCategories.aspx.cs" Inherits="RespondX.Admin.ManageCategories" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Admin.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-tags"></i> Manage Categories</h1>
            <p>Create, edit, and organize course categories</p>
        </div>

        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="filter-bar">
            <div class="filter-group">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search categories..." />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>
            <div class="filter-group">
                <asp:Button ID="btnAddCategory" runat="server" Text="+ Add Category" CssClass="btn btn-success" OnClick="btnAddCategory_Click" />
            </div>
        </div>

        <div class="modules-grid">
            <asp:Repeater ID="rptCategories" runat="server" OnItemCommand="rptCategories_ItemCommand">
                <ItemTemplate>
                    <div class="module-card admin-module-card">
                        <div class="module-header">
                            <h3><%# Eval("Name") %></h3>
                            <span class="badge <%# Convert.ToBoolean(Eval("IsActive")) ? "badge-success" : "badge-secondary" %>">
                                <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>
                            </span>
                        </div>
                        <p class="module-description"><%# Eval("Description") %></p>
                        <div class="module-meta">
                            <span><i class="fas fa-book"></i> <%# Eval("ModuleCount") %> Courses</span>
                        </div>
                        <div class="module-actions">
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="Edit" CommandArgument='<%# Eval("CategoryID") %>' />
                            <asp:Button ID="btnToggle" runat="server" Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate" : "Activate" %>' CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "btn btn-warning btn-sm" : "btn btn-success btn-sm" %>' CommandName="Toggle" CommandArgument='<%# Eval("CategoryID") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lblNoCategories" runat="server" Text="No categories found" Visible='<%# rptCategories.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <div class="modal fade" id="modalCategory" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Add Category"></asp:Label></h5>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hfCategoryID" runat="server" />
                        <div class="form-group">
                            <label>Name *</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <label>Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                        </div>
                        <div class="form-group">
                            <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" />
                            <label for="chkIsActive">Active</label>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="btnSaveCategory" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveCategory_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <style>
        .page-header { margin-bottom: 30px; }
        .page-header h1 { margin-bottom: 5px; }
        .page-header p { color: #718096; }
        .modules-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); gap: 20px; }
        .admin-module-card { background: white; border-radius: 10px; padding: 20px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-top: 4px solid #667eea; }
        .module-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 10px; }
        .module-header h3 { margin: 0; font-size: 18px; color: #2d3748; }
        .module-description { color: #718096; font-size: 14px; line-height: 1.6; margin-bottom: 12px; }
        .module-meta { display: flex; gap: 15px; font-size: 13px; color: #718096; margin-bottom: 12px; flex-wrap: wrap; }
        .module-actions { display: flex; gap: 8px; flex-wrap: wrap; }
        .filter-bar { display: flex; justify-content: space-between; flex-wrap: wrap; gap: 15px; margin-bottom: 20px; background: white; padding: 15px 20px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); }
        .filter-group { display: flex; gap: 10px; align-items: center; flex-wrap: wrap; }
    </style>
</asp:Content>
